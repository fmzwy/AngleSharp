﻿namespace AngleSharp.Html.Dom
{
    using AngleSharp.Dom;
    using AngleSharp.Dom.Services;
    using AngleSharp.Extensions;
    using AngleSharp.Io;
    using AngleSharp.Mathml.Dom;
    using AngleSharp.Svg.Dom;
    using AngleSharp.Text;
    using System;

    /// <summary>
    /// Represents a document node that contains only HTML nodes.
    /// </summary>
    sealed class HtmlDocument : Document, IHtmlDocument
    {
        #region Fields

        private readonly IElementFactory<HtmlElement> _htmlFactory;
        private readonly IElementFactory<MathElement> _mathFactory;
        private readonly IElementFactory<SvgElement> _svgFactory;

        #endregion

        #region ctor

        internal HtmlDocument(IBrowsingContext context, TextSource source)
            : base(context ?? BrowsingContext.New(), source)
        {
            ContentType = MimeTypeNames.Html;
            _htmlFactory = Context.GetFactory<IElementFactory<HtmlElement>>();
            _mathFactory = Context.GetFactory<IElementFactory<MathElement>>();
            _svgFactory = Context.GetFactory<IElementFactory<SvgElement>>();
        }

        internal HtmlDocument(IBrowsingContext context = null)
            : this(context, new TextSource(String.Empty))
        {
        }

        #endregion

        #region Properties

        public override IElement DocumentElement
        {
            get { return this.FindChild<HtmlHtmlElement>(); }
        }

        public override IEntityProvider Entities
        {
            get { return Context.GetProvider<IEntityProvider>() ?? HtmlEntityProvider.Resolver; }
        }

        #endregion

        #region Methods

        public override INode Clone(Boolean deep = true)
        {
            var source = new TextSource(Source.Text);
            var node = new HtmlDocument(Context, source);
            CloneDocument(node, deep);
            return node;
        }

        public HtmlElement CreateHtmlElement(String name, String prefix = null)
        {
            return _htmlFactory.Create(this, name, prefix);
        }

        public MathElement CreateMathElement(String name, String prefix = null)
        {
            return _mathFactory.Create(this, name, prefix);
        }

        public SvgElement CreateSvgElement(String name, String prefix = null)
        {
            return _svgFactory.Create(this, name, prefix);
        }

        internal override Element CreateElementFrom(String name, String prefix)
        {
            return CreateHtmlElement(name, prefix);
        }

        #endregion

        #region Helpers

        protected override String GetTitle()
        {
            var title = DocumentElement.FindDescendant<IHtmlTitleElement>();
            return title?.TextContent.CollapseAndStrip() ?? base.GetTitle();
        }

        protected override void SetTitle(String value)
        {
            var title = DocumentElement.FindDescendant<IHtmlTitleElement>();

            if (title == null)
            {
                var head = Head;

                if (head == null)
                {
                    return;
                }

                title = new HtmlTitleElement(this);
                head.AppendChild(title);
            }

            title.TextContent = value;
        }

        #endregion
    }
}
