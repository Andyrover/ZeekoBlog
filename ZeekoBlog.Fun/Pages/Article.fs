module ArticlePage

open ZeekoBlog.Core.Models
open Giraffe
open GiraffeViewEngine
open LayoutPage
open Common
open ZeekoBlog.Markdown.Plugins.TOCItemsPlugin
open ZeekoBlog.Markdown

type ArticleModel =
    { Article: Article
      Languages: string list
      TOCList: TOCItem list
      Markdown: MarkdownService
    }

let view (model: ArticleModel) =
    let markdown = createMd model.Markdown
    let layoutModel: LayoutModel =
        { Title = model.Article.Title }
    let sidebar =
        sideGroup "内容导航"
                  [ ul [ _class "toc-list" ]
                       ( model.TOCList
                         |> List.ofSeq
                         |> List.map
                            ( fun item ->
                                li [ _class (sprintf "toc-H%d" item.Level) ]
                                   [ a [ _href (sprintf "#%s" item.Id) ]
                                       [ rawText item.Name ]
                                   ]
                            )
                       )
                  ]
    let scripts =
        [ script [] [rawText "window.__pageModule = 'article-ro'"]
          script [ _src "/dist/article.js" ] []
          script []
                 [ rawText """
$('input[type=checkbox][disabled][checked]').replaceWith('<b class="mdl2" aria-hidden="true" style="font-size: .8rem;">&#xF16C;</b>')
$('input[type=checkbox][disabled]').replaceWith('<b class="mdl2" aria-hidden="true" style="font-size: .8rem;">&#xF16B;</b>');"""
                 ]
        ]
        @ mathJaxScript @
        [ script [ _src "https://cdn.bootcss.com/highlight.js/9.12.0/highlight.min.js" ] []
        ] @
        ( model.Languages
          |> List.map
            (fun lang ->
                script [ _src (sprintf "https://cdn.bootcss.com/highlight.js/9.12.0/languages/%s.min.js" lang ) ] []
            )
        )
    // view
    let viewBody =
        div [ _class "article" ]
            [ h1 [ _class "title" ] [ rawText model.Article.Title ]
              div []
                  [ span [ _class "weak" ] [ rawText (model.Article.LastEdited.ToString("yyyy/MM/dd")) ] ]
              div [ _class "divide wide-divide" ] []
              div [ _class "content article-md" ]
                  [ markdown model.Article.Content ]
            ]

    { Scripts = scripts
      Styles = []
      Body = [ viewBody ]
      Sidebar = [ sidebar ]
    }
    |> LayoutPage.view layoutModel