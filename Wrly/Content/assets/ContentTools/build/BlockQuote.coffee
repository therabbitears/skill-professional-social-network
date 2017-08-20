class ContentTools.Tools.BlockQuote extends ContentTools.Tools.Heading

    # Convert the current text block to a block quote (e.g <blockquote>foo</blockquote>)

    ContentTools.ToolShelf.stow(@, 'blockquote')

    @label = 'Block quote'
    @icon = 'blockquote'
    @tagName = 'blockquote'