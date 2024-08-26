# typed: true
# frozen_string_literal: true

require "sorbet-runtime"
require "dependabot/notices"
require "dependabot/package_manager"

# This module extracts helpers for notice generations that can be used
# for showing notices in logs, pr messages and alert ui page.
module Dependabot
  module NoticesHelpers
    extend T::Sig
    extend T::Helpers

    abstract!

    # Add a deprecation notice to the notice list if the package manager is deprecated
    # if the package manager is deprecated.
    #  notices << deprecation_notices if deprecation_notices
    sig do
      params(
        notices: T::Array[Dependabot::Notice],
        package_manager: T.nilable(PackageManagerBase)
      )
        .void
    end
    def add_deprecation_notice(notices:, package_manager:)
      # Create a deprecation notice if the package manager is deprecated
      deprecation_notice = create_deprecation_notice(package_manager)

      return unless deprecation_notice

      log_notice(deprecation_notice)

      notices << deprecation_notice
    end

    sig { params(notice: Dependabot::Notice).void }
    def log_notice(notice)
      # Log each non-empty line of the deprecation notice description
      notice.description.each_line do |line|
        line = line.strip
        next if line.empty?

        case notice.mode
        when Dependabot::Notice::NoticeMode::INFO
          return Dependabot.logger.info(line)
        when Dependabot::Notice::NoticeMode::WARN
          Dependabot.logger.warn(line)
        when Dependabot::Notice::NoticeMode::ERROR
          Dependabot.logger.error(line)
        else
          Dependabot.logger.info(line)
        end
      end
    end

    private

    sig { params(package_manager: T.nilable(PackageManagerBase)).returns(T.nilable(Dependabot::Notice)) }
    def create_deprecation_notice(package_manager)
      # Feature flag check if deprecation notice should be added to notices.
      return unless Dependabot::Experiments.enabled?(:add_deprecation_warn_to_pr_message)

      return unless package_manager

      return unless package_manager.is_a?(PackageManagerBase)

      Notice.generate_pm_deprecation_notice(
        package_manager
      )
    end
  end
end
