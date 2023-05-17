# frozen_string_literal: true

def common_dir
    @common_dir ||= Gem::Specification.find_by_name("dependabot-common").gem_dir
  end
  
  def require_common_spec(path)
    require "#{common_dir}/spec/dependabot/#{path}"
  end
  
  require "#{common_dir}/spec/spec_helper.rb"
  
  if ENV["COVERAGE"]
    # TODO: Bring branch coverage up
    SimpleCov.minimum_coverage line: 80, branch: 65
  end
  