using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable UnusedMember.Local
// ReSharper disable ConvertToAutoProperty

namespace Kontur.Search.Providers.InMemory
{
    public class TrieSearchProvider : IPrioritySearchProvider
    {
        private readonly object _lock = new object();
        private readonly Node _root = new Node(default(char), 0);

        private void Add([NotNull]string value, int priority)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            var length = value.Length;
            var node = _root;
            var localNode = (Node)null;

            for (int i = 0; i < length; i++)
            {
                var @char = value[i];

                localNode = node[@char];
                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (localNode == null)
                    localNode = node.AddChild(@char);

                node = localNode;
            }

            // ReSharper disable once PossibleNullReferenceException
            localNode.Priority = priority;
            localNode.FullValue = value;

            localNode.AddTransitional(localNode);
            var parentNode = localNode.Parent;

            while (parentNode != null && parentNode != _root)
            {
                parentNode.AddTransitional(localNode);
                parentNode = parentNode.Parent;
            }
        }

        public virtual void AddEntry(string entry)
        {
            AddEntry(entry, 0);
        }

        public virtual void AddEntry(string entry, int priority)
        {
            Monitor.Enter(_lock);
            try
            {
                Add(entry, priority);
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        [ItemNotNull]
        public virtual IEnumerable<string> Search(string query, int maxResults)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            Monitor.Enter(_lock);
            try
            {

                return SearchNodes(query, maxResults)
                    .Select(o => o.FullValue);
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<Node> SearchNodes([NotNull]string query, int maxResults)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            var node = _root;
            var length = query.Length;
            // ReSharper disable once TooWideLocalVariableScope
            var localNode = (Node)null;

            for (int i = 0; i < length; i++)
            {
                var @char = query[i];
                localNode = node[@char];

                if (localNode == null)
                    break;

                node = localNode;
            }

            if (node == _root)
                return Enumerable.Empty<Node>();

            return node.TransitionalNodes.Take(maxResults);
        }

        [DebuggerDisplay("Value = {Value,nq}, Priority = {Priority,nq}, Full Value = {FullValue,nq}")]
        private class Node
        {
            private readonly Hashtable _childNodes;
            // Why? Because this set internally use RB-Tree to maintain order
            // More info at http://referencesource.microsoft.com/#System/compmod/system/collections/generic/sortedset.cs,433
            private readonly SortedSet<Node> _transitionalNodes;
            private char _value;
            private int _priority;
            private string _fullValue;

            public Node Parent { get; private set; }

            public Node(char value, int priority)
            {
                _value = value;
                _priority = priority;
                _childNodes = new Hashtable();
                _transitionalNodes = new SortedSet<Node>(new NodeComparer());
            }

            [NotNull]
            public Hashtable ChildNodes => _childNodes;

            [NotNull]
            public IEnumerable<Node> TransitionalNodes => _transitionalNodes;


            public char Value
            {
                get
                {
                    return _value;
                }
                private set
                {
                    _value = value;
                }
            }

            public string FullValue
            {
                get
                {
                    return _fullValue;
                }
                set
                {
                    _fullValue = value;
                }
            }

            public int Priority
            {
                get
                {
                    return _priority;
                }
                set
                {
                    _priority = value;
                }
            }

            public Node AddChild(char value, int priority = 0)
            {
                var node = new Node(value, priority);
                node.Parent = this;
                _childNodes.Add(value, node);
                return node;
            }

            public void AddTransitional(Node node) => _transitionalNodes.Add(node);

            public Node this[char value] => _childNodes[value] as Node;
        }

        private class NodeComparer : IComparer<Node>
        {
            public int Compare(Node x, Node y)
            {
                if (x.Priority != y.Priority)
                    return x.Priority > y.Priority ? -1 : 1;
                if (!string.IsNullOrWhiteSpace(x.FullValue) && !string.IsNullOrWhiteSpace(y.FullValue))
                    return string.CompareOrdinal(x.FullValue, y.FullValue);

                return 0;
            }
        }
    }
}
