# Update Blog Selenium Script

## Dependencies
* [WP Githuber MD](https://github.com/terrylinooo/githuber-md) installed on WP site
* Wordpress site login credentials (needs to have access to edit)
* List of blog ID's that need to be updated

## What does this do
[WP Githuber MD](https://github.com/terrylinooo/githuber-md) creates a new content field in your wp_posts table for each post. If you want to mass convert all of your blogs to markdown by putting markdown in the new field in your database, it is still required that you press the update button on each blog for the plugin to actually convert the markdown into html that will be presented when somebody hits the page. This will go to every blog (if given the post ID) and manually press the update button.

