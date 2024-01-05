# frozen_string_literal: true

Gem::Specification.new do |spec|
  common_gemspec =
    Bundler.load_gemspec_uncached("../common/dependabot-common.gemspec")

  spec.name         = "dependabot-npm_and_yarn"
  spec.summary      = "Provides Dependabot support for Javascript (npm and yarn)"
  spec.description  = "Dependabot-NPM_And_Yarn provides support for bumping Javascript (npm and yarn) libraries via " \
                      "Dependabot. "

  spec.author       = common_gemspec.author
  spec.email        = common_gemspec.email
  spec.homepage     = common_gemspec.homepage
  spec.license      = common_gemspec.license

  spec.metadata = {
    "bug_tracker_uri" => common_gemspec.metadata["bug_tracker_uri"],
    "changelog_uri" => common_gemspec.metadata["changelog_uri"]
  }

  spec.version = common_gemspec.version
  spec.required_ruby_version = common_gemspec.required_ruby_version
  spec.required_rubygems_version = common_gemspec.required_ruby_version

  spec.require_path = "lib"
  spec.files        = []

  spec.add_dependency "dependabot-common", Dependabot::VERSION

  common_gemspec.development_dependencies.each do |dep|
    spec.add_development_dependency dep.name, *dep.requirement.as_list
  end

  next unless File.exist?("../.gitignore")

  spec.files += `git -C #{__dir__} ls-files lib helpers -z`.split("\x0")
end
