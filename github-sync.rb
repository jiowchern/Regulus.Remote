#!/usr/bin/env ruby

require 'net/http'
require 'uri'
require 'yaml'
require 'json'

outdir = "github/_posts"
Dir.mkdir(outdir) unless File.exists?(outdir)

config = YAML.load_file('_config.yml')
username = config['github_username']
repos = JSON.parse(Net::HTTP.get(URI.parse(URI.encode("https://api.github.com/users/#{username}/repos"))))
repos.each { |r|
  reponame = r['name']
  readme = Net::HTTP.get(URI.parse(URI.encode("https://raw.githubusercontent.com/#{username}/#{reponame}/master/README.md")))
  if readme != "Not Found"
    if readme[0,3] == "---"
      # update frontmatter to add in reponame and layout params
      fmEnd = readme.index(/^---/, 4)
      if fmEnd
        fm = YAML.load(readme[3,fmEnd])
        if fm['date']
          content = """---
reponame: #{reponame}
layout: repo""" + readme[3..-1]
          reponame.gsub!('.','_')
          outfile = "#{outdir}/#{fm['date']}-#{reponame}.md"
          File.open(outfile, 'w') {|f| f.write(content) }
          puts "Wrote #{outfile}"
        else
          puts "Ignored #{reponame}: README.md frontmatter missing 'date'"
        end
      else
        puts "Ignored #{reponame}: README.md frontmatter end not found"
      end
    else
      puts "Ignored #{reponame}: README.md contains no frontmatter"
    end
  else
    puts "Ignored #{reponame}: README.md not found"
  end
}
