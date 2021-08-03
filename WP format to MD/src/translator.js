const turndown = require('turndown');
const turndownPluginGfm = require('turndown-plugin-gfm');

function initTurndownService() {
	const turndownService = new turndown({
		headingStyle: 'atx',
		bulletListMarker: '-',
		codeBlockStyle: 'fenced'
	});

	turndownService.use(turndownPluginGfm.tables);

	// preserve embedded tweets
	turndownService.addRule('tweet', {
		filter: node => node.nodeName === 'BLOCKQUOTE' && node.getAttribute('class') === 'twitter-tweet',
		replacement: (content, node) => '\n\n' + node.outerHTML
	});

	// preserve embedded codepens
	turndownService.addRule('codepen', {
		filter: node => {
			// codepen embed snippets have changed over the years
			// but this series of checks should find the commonalities
			return (
				['P', 'DIV'].includes(node.nodeName) &&
				node.attributes['data-slug-hash'] &&
				node.getAttribute('class') === 'codepen'
			);
		},
		replacement: (content, node) => '\n\n' + node.outerHTML
	});

	// preserve embedded scripts (for tweets, codepens, gists, etc.)
	turndownService.addRule('script', {
		filter: 'script',
		replacement: (content, node) => {
			let before = '\n\n';
			if (node.previousSibling && node.previousSibling.nodeName !== '#text') {
				// keep twitter and codepen <script> tags snug with the element above them
				before = '\n';
			}
			const html = node.outerHTML.replace('async=""', 'async');
			return before + html + '\n\n';
		}
	});
	/*
	[
  'parentNode',    '_previousSibling',
  '_nextSibling',  '_index',
  '_childNodes',   '_firstChild',
  'nodeType',      'ownerDocument',
  'localName',     'namespaceURI',
  'prefix',        '_tagName',
  '_attrsByQName', '_attrsByLName',
  '_attrKeys',     '_nid',
  'isBlock',       'isCode',
  'isBlank',       'flankingWhitespace'
]*/

	turndownService.addRule('code', {
		filter: function (node, options) {
			if(node.className != "")
				console.log(node.className)
			return (
			  node.className == "wp-block-syntaxhighlighter-code" ||
			  node.className == "wp-block-code"
			)
		  },
		replacement: function (content) {
			return '```\n' + content + '\n```'
		}
	})

	turndownService.addRule('oldCode', {
		filter: function (node, options) {
			// console.log(node.parentNode)
			// console.log(node.isCode)
			// console.log(node.localName)
			console.log(node._tagName)
			var oldCode = false;
			var listElement = false;
			var tempNode = node;
			try{
				// while(!(tempNode._previousSibling === undefined)){
				// 	if(tempNode.parentNode._tagName == 'LI' || tempNode.parentNode._tagName == 'OL' || tempNode.parentNode._tagName == 'UL'){
				// 		listElement = true;
				// 	}
				// 	tempNode = node._previousSibling;
				// 	console.log(tempNode)
				// }
				if(tempNode.parentNode._tagName == 'LI' || tempNode.parentNode._tagName == 'OL' || tempNode.parentNode._tagName == 'UL'){
					listElement = true;
				}
			} catch(err){
				console.log(err)
			}

			if(!listElement){
			try { 
				var string = ""
				for(i = 0; i < node._attrsByQName.style.data.length; i++){
					string += node._attrsByQName.style.data[i]
				}
				console.log(string)
				console.log("\n")
				if(string.includes("Courier New") && node._tagName != 'LI'){
					oldCode = true
				}
				//console.log(Object.getOwnPropertyNames(node._attrsByQName.style.data));

			}
			catch (err){
				console.log(err)
			}
		}

			if(node.className != "")
				console.log(node.className)
			return (
			  oldCode
			)
		  },
		replacement: function (content) {
			console.log(`${content}\n`)
			if(!content.includes("```")){
				return '``` ' + content + ' ```'
			}
			return content
		}
	})
	// preserve iframes (common for embedded audio/video)
	turndownService.addRule('iframe', {
		filter: 'iframe',
		replacement: (content, node) => {
			const html = node.outerHTML.replace('allowfullscreen=""', 'allowfullscreen');
			return '\n\n' + html + '\n\n';
		}
	});

	return turndownService;
}

function getPostContent(post, turndownService, config) {
	let content = post.encoded[0];

	// insert an empty div element between double line breaks
	// this nifty trick causes turndown to keep adjacent paragraphs separated
	// without mucking up content inside of other elemnts (like <code> blocks)
	content = content.replace(/(\r?\n){2}/g, '\n<div></div>\n');

	if (config.saveScrapedImages) {
		// writeImageFile() will save all content images to a relative /images
		// folder so update references in post content to match
		content = content.replace(/(<img[^>]*src=").*?([^/"]+\.(?:gif|jpe?g|png))("[^>]*>)/gi, '$1images/$2$3');
	}

	// this is a hack to make <iframe> nodes non-empty by inserting a "." which
	// allows the iframe rule declared in initTurndownService() to take effect
	// (using turndown's blankRule() and keep() solution did not work for me)
	content = content.replace(/(<\/iframe>)/gi, '.$1');

	// use turndown to convert HTML to Markdown
	content = turndownService.turndown(content);

	// clean up extra spaces in list items
	content = content.replace(/(-|\d+\.) +/g, '$1 ');

	// clean up the "." from the iframe hack above
	content = content.replace(/\.(<\/iframe>)/gi, '$1');

	return content;
}

exports.initTurndownService = initTurndownService;
exports.getPostContent = getPostContent;
