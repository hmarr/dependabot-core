# typed: strict
# frozen_string_literal: true

require "base64"
require "dependabot/base_command"
require "dependabot/dependency_snapshot"
require "dependabot/errors"
require "dependabot/opentelemetry"
require "dependabot/updater"

require "sorbet-runtime"

module Dependabot
  class UpdateFilesCommand < BaseCommand
    extend T::Sig

    sig { override.void }
    def perform_job
      # We expect the FileFetcherCommand to have been executed beforehand to place
      # encoded files and commit information in the environment, so let's retrieve
      # them, decode and parse them into an object that knows the current state
      # of the project's dependencies.
      ::Dependabot::OpenTelemetry.tracer.in_span("update_files", kind: :internal) do |span|
        span.set_attribute(::Dependabot::OpenTelemetry::Attributes::JOB_ID, job_id.to_s)

        begin
          dependency_snapshot = Dependabot::DependencySnapshot.create_from_job_definition(
            job: job,
            job_definition: Environment.job_definition
          )
        rescue StandardError => e
          handle_parser_error(e)
          # If dependency file parsing has failed, there's nothing more we can do,
          # so let's mark the job as processed and stop.
          return service.mark_job_as_processed(base_commit_sha)
        end

        # Update the service's metadata about this project
        service.update_dependency_list(dependency_snapshot: dependency_snapshot)

        # TODO: Pull fatal error handling handling up into this class
        #
        # As above, we can remove the responsibility for handling fatal/job halting
        # errors from Dependabot::Updater entirely.
        Dependabot::Updater.new(
          service: service,
          job: job,
          dependency_snapshot: dependency_snapshot
        ).run

        # Wait for all PRs to be created
        service.wait_for_calls_to_finish

        # Finally, mark the job as processed. The Dependabot::Updater may have
        # reported errors to the service, but we always consider the job as
        # successfully processed unless it actually raises.
        service.mark_job_as_processed(dependency_snapshot.base_commit_sha)
      end
    end

    private

    sig { override.returns(Job) }
    def job
      @job ||= T.let(
        Job.new_update_job(
          job_id: job_id,
          job_definition: Environment.job_definition,
          repo_contents_path: Environment.repo_contents_path
        ), T.nilable(Job)
      )
    end

    sig { override.returns(String) }
    def base_commit_sha
      Environment.job_definition["base_commit_sha"]
    end

    # rubocop:disable Metrics/AbcSize, Metrics/PerceivedComplexity
    sig { params(error: StandardError).void }
    def handle_parser_error(error)
      # This happens if the repo gets removed after a job gets kicked off.
      # The service will handle the removal without any prompt from the updater,
      # so no need to add an error to the errors array
      return if error.is_a? Dependabot::RepoNotFound

      error_details = Dependabot.parser_error_details(error)

      error_details ||=
        # Check if the error is a known "run halting" state we should handle
        if (error_type = Updater::ErrorHandler::RUN_HALTING_ERRORS[error.class])
          { "error-type": error_type }
        else
          # If it isn't, then log all the details and let the application error
          # tracker know about it
          Dependabot.logger.error error.message
          error.backtrace&.each { |line| Dependabot.logger.error line }
          sentry_context = error.respond_to?(:sentry_context) ? T.unsafe(error).sentry_context[:fingerprint] : nil
          unknown_error_details = {
            ErrorAttributes::CLASS => error.class.to_s,
            ErrorAttributes::MESSAGE => error.message,
            ErrorAttributes::BACKTRACE => error.backtrace&.join("\n"),
            ErrorAttributes::FINGERPRINT => sentry_context,
            ErrorAttributes::PACKAGE_MANAGER => job.package_manager,
            ErrorAttributes::JOB_ID => job.id,
            ErrorAttributes::DEPENDENCIES => job.dependencies,
            ErrorAttributes::DEPENDENCY_GROUPS => job.dependency_groups
          }.compact

          service.capture_exception(error: error, job: job)

          # Set an unknown error type as update_files_error to be added to the job
          {
            "error-type": "update_files_error",
            "error-detail": unknown_error_details
          }
        end

      service.record_update_job_error(
        error_type: error_details.fetch(:"error-type"),
        error_details: error_details[:"error-detail"]
      )
      # We don't set this flag in GHES because there older GHES version does not support reporting unknown errors.
      return unless Experiments.enabled?(:record_update_job_unknown_error)
      return unless error_details.fetch(:"error-type") == "update_files_error"

      service.record_update_job_unknown_error(
        error_type: error_details.fetch(:"error-type"),
        error_details: error_details[:"error-detail"]
      )
    end
    # rubocop:enable Metrics/AbcSize, Metrics/PerceivedComplexity
  end
end
