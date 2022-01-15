using System;
using System.Collections;
using System.Collections.Generic;

namespace Generics.BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        private BinaryTree<T> left;

        public BinaryTree<T> Left
        {
            get
            {
                if (left == null)
                    left = new BinaryTree<T>();
                return left;
            }
        }

        private BinaryTree<T> right;

        public BinaryTree<T> Right
        {
            get
            {
                if (right == null)
                    right = new BinaryTree<T>();
                return right;
            }
        }

        public BinaryTree<T> Parent { get; set; }
        public T Value { get; set; }
        public bool HasValue { get; set; }

        public BinaryTree()
        {
            HasValue = false;
        }

        public void Add(T value)
        {
            if (!HasValue)
            {
                Value = value;
                HasValue = true;
                Parent = null;
            }
            else
            {
                if (value.CompareTo(Value) == 1)
                    Insert(value, Right, this);
                else
                    Insert(value, Left, this);
            }
        }

        private void Insert(T value, BinaryTree<T> currentNode, BinaryTree<T> parent)
        {
            if (!currentNode.HasValue)
            {
                currentNode.Value = value;
                currentNode.Parent = parent;
                currentNode.HasValue = true;
                return;
            }

            var comparisonValue = value.CompareTo(currentNode.Value);
            if (comparisonValue == 1)
                Insert(value, currentNode.Right, currentNode);
            else if (comparisonValue == -1)
                Insert(value, currentNode.Left, currentNode);
        }

        public IEnumerator<T> GetEnumerator() => GetEnumeratorForNode(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T> GetEnumeratorForNode(BinaryTree<T> tree)
        {
            if (tree == null || !tree.HasValue)
                yield break;

            var enumeratorForTreeNode = GetEnumeratorForNode(tree.Left);
            while (enumeratorForTreeNode.MoveNext())
                yield return enumeratorForTreeNode.Current;

            yield return tree.Value;
            enumeratorForTreeNode = GetEnumeratorForNode(tree.Right);
            while (enumeratorForTreeNode.MoveNext())
                yield return enumeratorForTreeNode.Current;
        }
    }

    public static class BinaryTree
    {
        public static BinaryTree<int> Create(params int[] items)
        {
            var binaryTree = new BinaryTree<int>();
            foreach (var item in items)
                binaryTree.Add(item);

            return binaryTree;
        }
    }
}