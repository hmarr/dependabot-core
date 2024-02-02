# typed: true
# frozen_string_literal: true

# Go pre-release versions use 1.0.1-rc1 syntax, which Gem::Version
# converts into 1.0.1.pre.rc1. We override the `to_s` method to stop that
# alteration.
# Best docs are at https://github.com/Masterminds/semver

require "dependabot/version"
require "dependabot/utils"

module Dependabot
  module GoModules
    class Version < Dependabot::Version
      VERSION_PATTERN = '[0-9]+[0-9a-zA-Z]*(?>\.[0-9a-zA-Z]+)*' \
                        '(-[0-9A-Za-z-]+(\.[0-9a-zA-Z-]+)*)?' \
                        '(\+incompatible)?'
      ANCHORED_VERSION_PATTERN = /\A\s*(#{VERSION_PATTERN})?\s*\z/

      def self.correct?(version)
        version = version.gsub(/^v/, "") if version.is_a?(String)
        version = version.to_s.split("+").first if version.to_s.include?("+")

        super(version)
      end

      def initialize(version)
        @version_string = version.to_s.gsub(/^v/, "")
        version = version.gsub(/^v/, "") if version.is_a?(String)
        version = version.to_s.split("+").first if version.to_s.include?("+")
        version, @prerelease = version.to_s.split("-") if version.to_s.include?("-")

        super
      end

      def inspect # :nodoc:
        "#<#{self.class} #{@version_string.inspect}>"
      end

      def to_s
        @version_string
      end

      def <=>(other)
        result = super(other)
        return if result.nil?
        return result unless result.zero?

        other = self.class.new(other) unless other.is_a?(Version)
        compare_prerelease(@prerelease || "", other.prerelease || "")
      end

      protected

      attr_reader :prerelease

      private

      # This matches Go's semver behavior
      # see https://github.com/golang/mod/blob/fa1ba4269bda724bb9f01ec381fbbaf031e45833/semver/semver.go#L333
      def compare_prerelease(left, right)
        return 0 if left == right
        return 1 if left == ""
        return -1 if right == ""

        while left != "" && right != ""
          left = left[1..-1] if left.start_with?(".", "-")
          right = right[1..-1] if right.start_with?(".", "-")

          dx, left = next_ident(left)
          dy, right = next_ident(right)
          if dx != dy
            ix = is_num(dx)
            iy = is_num(dy)
            if ix != iy
              if ix
                return -1
              else
                return 1
              end
            end
            if ix
              if dx.length < dy.length
                return -1
              end
              if dx.length > dy.length
                return 1
              end
            end
            if dx < dy
              return -1
            else
              return 1
            end
          end
        end

        if left == ""
          return -1
        else
          return 1
        end
      end

      def next_ident(data)
        i = 0
        i += 1 while i < data.length && data[i] != "."
        [data[0..i], data[i..-1]]
      end

      def is_num(data)
        i = 0
        while i < data.length && "0" <= data[i] && data[i] <= "9"
          i += 1
        end
        return i == data.length
      end
    end
  end
end

Dependabot::Utils
  .register_version_class("go_modules", Dependabot::GoModules::Version)
