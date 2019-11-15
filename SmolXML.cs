using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmolXML
{
    enum ParsingState
    {
        NonXML,
        StringLiteral,
        InsideTag,
        InsideValue
    }

    public class SmolRootElement : SmolElement
    {
        public SmolRootElement() :
            base(null)
        {
        }
    }
    public class SmolTextElement : SmolElement
    {
        public string text = "";

        public SmolTextElement(SmolElement parent) :
            base(parent)
        {
        }

        public override void Dump(int depth = 0)
        {
            for (int i = 0; i < depth; i++) Console.Write("  ");
            Console.WriteLine("text: " + text);
        }
    }
    public class SmolElement
    {
        public string tagName = "";
        public SmolElement parent;

        private List<SmolElement> _children = new List<SmolElement>();
        public SmolElement[] children => _children.ToArray();

        public SmolElement(SmolElement parent)
        {
            this.parent = parent;
        }

        public void AddChild(SmolElement elem)
        {
            _children.Add(elem);
        }

        public virtual void Dump(int depth = 0)
        {
            for (int i = 0; i < depth; i++) Console.Write("  ");
            Console.WriteLine(tagName);

            foreach (var c in children)
                c.Dump(depth + 1);
        }
    }
    public class SmolXMLParser
    {
        private ParsingState state = ParsingState.NonXML;
        private int xmlLevel = 0;
        private int quoteLevel = 0;

        private SmolElement elem = new SmolRootElement();
        private StringBuilder buffer = new StringBuilder();

        public SmolRootElement Parse(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '"')
                {
                    if (quoteLevel == 1)
                        quoteLevel = 0;
                    else
                        quoteLevel = 1;
                }

                if (input[i] == '<')
                {
                    state = ParsingState.InsideTag;

                    if (buffer.Length != 0)
                    {
                        elem.AddChild(new SmolTextElement(elem)
                        {
                            text = buffer.ToString()
                        });
                        buffer.Clear();
                    }

                    if (input[i + 1] == '/')
                    {
                        xmlLevel--;
                    }
                    else
                    {
                        var e = new SmolElement(elem);
                        elem.AddChild(e);

                        elem = e;

                        xmlLevel++;
                    }
                }
                else if (input[i] == '>')
                {
                    if (xmlLevel == 0)
                        state = ParsingState.NonXML;
                    else
                        state = ParsingState.InsideValue;

                    if (buffer[0] == '/')
                        elem = elem.parent;
                    else
                        elem.tagName = buffer.ToString();
                    buffer.Clear();
                }
                else
                {
                    buffer.Append(input[i]);
                }
            }

            return elem as SmolRootElement;
        }
    }
}
