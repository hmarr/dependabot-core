# typed: false
# frozen_string_literal: true

require "dependabot/npm_and_yarn/package_manager"
require "dependabot/npm_and_yarn/helpers"
require "spec_helper"

RSpec.describe Dependabot::NpmAndYarn::PackageManagerHelper do
  let(:npm_lockfile) do
    instance_double(
      Dependabot::DependencyFile,
      name: "package-lock.json",
      content: <<~LOCKFILE
        {
          "name": "example-npm-project",
          "version": "1.0.0",
          "lockfileVersion": 2,
          "requires": true,
          "dependencies": {
            "lodash": {
              "version": "4.17.21",
              "resolved": "https://registry.npmjs.org/lodash/-/lodash-4.17.21.tgz",
              "integrity": "sha512-abc123"
            }
          }
        }
      LOCKFILE
    )
  end

  let(:yarn_lockfile) do
    instance_double(
      Dependabot::DependencyFile,
      name: "yarn.lock",
      content: <<~LOCKFILE
        # THIS IS AN AUTOGENERATED FILE. DO NOT EDIT THIS FILE DIRECTLY.
        # yarn lockfile v1

        lodash@^4.17.20:
          version "4.17.21"
          resolved "https://registry.yarnpkg.com/lodash/-/lodash-4.17.21.tgz#abc123"
          integrity sha512-abc123
      LOCKFILE
    )
  end

  let(:pnpm_lockfile) do
    instance_double(
      Dependabot::DependencyFile,
      name: "pnpm-lock.yaml",
      content: <<~LOCKFILE
        lockfileVersion: 5.4

        dependencies:
          lodash:
            specifier: ^4.17.20
            version: 4.17.21
            resolution:
              integrity: sha512-abc123
              tarball: https://registry.npmjs.org/lodash/-/lodash-4.17.21.tgz
      LOCKFILE
    )
  end

  let(:lockfiles) { { npm: npm_lockfile, yarn: yarn_lockfile, pnpm: pnpm_lockfile } }
  let(:package_json) { { "packageManager" => "npm@7" } }
  let(:helper) { described_class.new(package_json, lockfiles: lockfiles) }

  describe "#package_manager" do
    context "when npm lockfile exists" do
      it "returns an NpmPackageManager instance" do
        allow(Dependabot::NpmAndYarn::Helpers).to receive(:npm_version_numeric).and_return(7)
        expect(helper.package_manager).to be_a(Dependabot::NpmAndYarn::NpmPackageManager)
      end
    end

    context "when only yarn lockfile exists" do
      let(:lockfiles) { { yarn: yarn_lockfile } }

      it "returns a YarnPackageManager instance" do
        allow(Dependabot::NpmAndYarn::Helpers).to receive(:yarn_version_numeric).and_return(1)
        expect(helper.package_manager).to be_a(Dependabot::NpmAndYarn::YarnPackageManager)
      end
    end

    context "when only pnpm lockfile exists" do
      let(:lockfiles) { { pnpm: pnpm_lockfile } }

      it "returns a PNPMPackageManager instance" do
        allow(Dependabot::NpmAndYarn::Helpers).to receive(:pnpm_version_numeric).and_return(7)
        expect(helper.package_manager).to be_a(Dependabot::NpmAndYarn::PNPMPackageManager)
      end
    end

    context "when no lockfile but packageManager attribute exists" do
      let(:lockfiles) { {} }

      it "returns an NpmPackageManager instance based on the packageManager attribute" do
        expect(helper.package_manager).to be_a(Dependabot::NpmAndYarn::NpmPackageManager)
      end
    end

    context "when no lockfile and packageManager attribute, but engines field exists" do
      let(:lockfiles) { {} }
      let(:package_json) { { "engines" => { "yarn" => "1" } } }

      it "returns a YarnPackageManager instance from engines field" do
        expect(helper.package_manager).to be_a(Dependabot::NpmAndYarn::YarnPackageManager)
      end
    end

    context "when neither lockfile, packageManager, nor engines field exists" do
      let(:lockfiles) { {} }
      let(:package_json) { {} }

      it "returns default package manager" do
        expect(helper.package_manager).to be_a(Dependabot::NpmAndYarn::NpmPackageManager)
      end
    end
  end

  describe "#installed_version" do
    before do
      allow(Dependabot::NpmAndYarn::Helpers).to receive_messages(
        npm_version_numeric: 7,
        yarn_version_numeric: 1,
        pnpm_version_numeric: 7
      )
    end

    context "when the installed version matches the expected format" do
      before do
        allow(Dependabot::SharedHelpers).to receive(:run_shell_command)
          .with("npm --version", fingerprint: "<name> --version").and_return("7.5.2")
      end

      it "returns the raw installed version" do
        expect(helper.installed_version("npm")).to eq("7.5.2")
      end
    end

    context "when the installed version does not match the expected format" do
      before do
        allow(Dependabot::SharedHelpers).to receive(:run_shell_command)
          .with("yarn --version", fingerprint: "<name> --version")
          .and_return("invalid_version")
        allow(Dependabot::NpmAndYarn::Helpers).to receive(:yarn_version_numeric)
          .and_return(1)
      end

      it "falls back to the lockfile version" do
        expect(helper.installed_version("yarn")).to eq("1")
        # Check that the fallback version is memoized
        expect(helper.instance_variable_get(:@installed_versions)["yarn"]).to eq("1")
      end
    end

    context "when memoization is in effect" do
      before do
        allow(Dependabot::SharedHelpers).to receive(:run_shell_command)
          .with("pnpm --version", fingerprint: "<name> --version").and_return("7.1.0")
        # Pre-cache the result
        helper.installed_version("pnpm")
      end

      it "does not re-run the shell command and uses the cached version" do
        expect(Dependabot::SharedHelpers).not_to receive(:run_shell_command)
          .with("pnpm --version", fingerprint: "<name> --version")
        expect(helper.installed_version("pnpm")).to eq("7.1.0")
      end
    end
  end
end
