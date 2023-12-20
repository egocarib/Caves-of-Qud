using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XRL.World.Conversations
{

    public static class ConversationElementExtensions
    {
        public static List<Node> VeAisse_GetNodes(this IConversationElement elem)
        {
            return GetChildOfType<Node>(elem);
        }

        public static List<Choice> VeAisse_GetChoices(this IConversationElement elem)
        {
            return GetChildOfType<Choice>(elem);
        }

        private static List<T> GetChildOfType<T>(IConversationElement elem) where T : IConversationElement
        {
            List<T> res = new List<T>();
            
            foreach(IConversationElement child in elem.Elements)
                if(child is T t) res.Add(t);

            return res;
        }
    }
}