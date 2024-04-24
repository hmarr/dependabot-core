# typed: true

# DO NOT EDIT MANUALLY
# This is an autogenerated file for types exported from the `commonmarker` gem.
# Please instead update this file by running `bin/tapioca gem commonmarker`.

# source://commonmarker//lib/commonmarker/constants.rb#3
module Commonmarker
  private

  def commonmark_to_html(*_arg0); end

  class << self
    def commonmark_to_html(*_arg0); end

    # Public: Parses a CommonMark string into an HTML string.
    #
    # text - A {String} of text
    # options - A {Hash} of render, parse, and extension options to transform the text.
    # plugins - A {Hash} of additional plugins.
    #
    # Returns a {String} of converted HTML.
    #
    # @raise [TypeError]
    #
    # source://commonmarker//lib/commonmarker.rb#19
    def to_html(text, options: T.unsafe(nil), plugins: T.unsafe(nil)); end
  end
end

# source://commonmarker//lib/commonmarker/config.rb#4
module Commonmarker::Config
  extend ::Commonmarker::Constants
  extend ::Commonmarker::Utils

  class << self
    # source://commonmarker//lib/commonmarker/config.rb#46
    def merged_with_defaults(options); end

    # source://commonmarker//lib/commonmarker/config.rb#66
    def process_extension_options(option); end

    # source://commonmarker//lib/commonmarker/config.rb#50
    def process_options(options); end

    # source://commonmarker//lib/commonmarker/config.rb#66
    def process_parse_options(option); end

    # source://commonmarker//lib/commonmarker/config.rb#58
    def process_plugins(plugins); end

    # source://commonmarker//lib/commonmarker/config.rb#66
    def process_render_options(option); end

    # source://commonmarker//lib/commonmarker/config.rb#82
    def process_syntax_highlighter_plugin(plugin); end
  end
end

# For details, see
# https://github.com/kivikakk/comrak/blob/162ef9354deb2c9b4a4e05be495aa372ba5bb696/src/main.rs#L201
#
# source://commonmarker//lib/commonmarker/config.rb#7
Commonmarker::Config::OPTIONS = T.let(T.unsafe(nil), Hash)

# source://commonmarker//lib/commonmarker/config.rb#36
Commonmarker::Config::PLUGINS = T.let(T.unsafe(nil), Hash)

# source://commonmarker//lib/commonmarker/constants.rb#4
module Commonmarker::Constants; end

# source://commonmarker//lib/commonmarker/constants.rb#5
Commonmarker::Constants::BOOLS = T.let(T.unsafe(nil), Array)

# source://commonmarker//lib/commonmarker/renderer.rb#7
class Commonmarker::Renderer; end

# source://commonmarker//lib/commonmarker/utils.rb#6
module Commonmarker::Utils
  include ::Commonmarker::Constants

  # source://commonmarker//lib/commonmarker/utils.rb#9
  def fetch_kv(option, key, value, type); end
end

# source://commonmarker//lib/commonmarker/version.rb#4
Commonmarker::VERSION = T.let(T.unsafe(nil), String)
