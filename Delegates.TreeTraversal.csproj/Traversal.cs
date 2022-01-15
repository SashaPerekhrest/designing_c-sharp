using System;
using System.Collections.Generic;

namespace Delegates.TreeTraversal
{
    public class Traversal
    {
        private delegate void TraverseDelegate<T, TIntermediate>(
        T data, Func<T, bool> hasValue, List<TIntermediate> list);
 
        private static IEnumerable<TIntermediate> Traverse<T, TIntermediate>(
            T data, TraverseDelegate<T, TIntermediate> traverse, Func<T, bool> hasValue)
        {
            var list = new List<TIntermediate>();
            traverse(data, hasValue, list);
            return list;
        }
      
        private static void TraverseProductCategory(
            ProductCategory data, Func<ProductCategory, bool> hasValue, List<Product> list)
        {
            if (data == null) return;
            if (hasValue(data))
                list.AddRange(data.Products);

            foreach (var category in data.Categories)
                TraverseProductCategory(category, hasValue, list);
        }
        
        private static void TraverseJob(Job data, Func<Job, bool> hasValue, List<Job> list)
        {
            if (data == null) return;
            if (hasValue(data))
                list.Add(data);

            foreach (var subjob in data.Subjobs)
                TraverseJob(subjob, hasValue, list);
        }
        
        private static void TraverseBinaryTree<TIntermediate>(
            BinaryTree<TIntermediate> data, Func<BinaryTree<TIntermediate>, bool> hasValue, List<TIntermediate> list)
        {
            if (data == null) return;
            if (hasValue(data))
                list.Add(data.Value);

            TraverseBinaryTree(data.Left, hasValue, list);
            TraverseBinaryTree(data.Right, hasValue, list);
        }
        
        public static IEnumerable<int> GetBinaryTreeValues(BinaryTree<int> data) 
            => Traverse<BinaryTree<int>, int>(
                data, 
                TraverseBinaryTree, 
                hasValue => hasValue.Left == null && hasValue.Right == null);
        
 
        public static IEnumerable<Job> GetEndJobs(Job data) 
            => Traverse<Job, Job>(data, TraverseJob, hasValue => hasValue.Subjobs.Count == 0);

        public static IEnumerable<Product> GetProducts(ProductCategory data) 
            => Traverse<ProductCategory, Product>(data, TraverseProductCategory, hasValue => true);
    }
}