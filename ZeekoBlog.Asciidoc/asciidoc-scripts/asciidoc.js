const Asciidoctor = require('asciidoctor.js');
const asciidoctor = Asciidoctor();
const cheerio = require('cheerio');
const distinct = require('lodash.uniq');
const highlightJsExt = require('asciidoctor-highlight.js')

const registry = asciidoctor.Extensions.create();
highlightJsExt.register(registry);


/**
 * @typedef {{source:string, value: string, languages: string[], tableOfContents:ITOCItem[]}} IRenderedResult
 */

/**
 * @typedef {{name: string, level: number, id:string}} ITOCItem
 */

/**
 * @param {ITOCItem} obj
 * @returns {ITOCItem}
 */
const tocOf = (obj) => {
    return obj;
}

/**
 * 
 * @param {IRenderedResult} obj
 * @returns {IRenderedResult}
 */
const renderedOf = (obj) => {
    return obj;
}


const convertOptions = {
    safe: 'safe',
    attributes: {
        icons: 'font',
        stem: 'latexmath',
        toc: 'auto',
        'toc-title': '内容导航',
        toclevels: 5,
        linkattrs: '',
        'caution-caption': '⚠️',
        'important-caption': '‼️',
        'note-caption': '💬',
        'tip-caption': '💡',
        'warning-caption': '🚨',
        'source-highlighter': 'highlightjs-ext'
    },
    'extension_registry': registry,
};

/**
 * 解析文章目录
 *
 * @param {Cheerio} level1ul 第一级标题
 * @param {CheerioStatic} $
 */
function extractToc(level1ul, $) {
    /**
     * @type {[number, CheerioElement][]}
     */
    const result = [];
    /**
     * @type {[number, CheerioElement][]}
     */
    let stack = [];
    stack.push(...level1ul.children('li').toArray().map(li => [2, li]));
    while (stack.length > 0) {
        const current = stack[0];
        result.push(current);
        stack.shift();
        const children = $(current[1]).children('ul').children('li');
        if (children.length > 0) {
            const childLevel = current[0] + 1;
            stack.unshift(...children.toArray().map(li => [childLevel, li]));
        }
    }
    const tocList = result.map(([level, li]) => {
        const anchor = $(li).children('a');
        const name = anchor.text();
        const anchorName = anchor.attr('href').slice(1);
        return tocOf({
            name, anchorName, level
        });
    });
    return tocList;
}

function render(callback, source, bypass) {
    /**
     * @type {string []}
     */
    try {
        // debugger;
        const result = asciidoctor.convert(source, convertOptions);
        const tmp = `<div>${result}</div>`;
        const $ = cheerio.load(tmp, { xmlMode: true, decodeEntities: false });
        // detect languages
        const codeBlocks = $('pre code[data-lang]');
        const languages = distinct(codeBlocks.map((_, el) => $(el).data('lang')).toArray());
        // get table of content
        const level1 = $('#toc > ul.sectlevel1');
        let toc = [];
        if (level1.length !== 0) {
            toc = extractToc(level1, $);
            $('#toc').remove();
        }
        const rendered = renderedOf({
            source, languages,
            value: result,
            tableOfContents: toc
        });
        callback(null, rendered);
        return;
    } catch (e) {
        callback(e, renderedOf({
            source, languages: [], value: source
        }));
    }
}

module.exports = render;
