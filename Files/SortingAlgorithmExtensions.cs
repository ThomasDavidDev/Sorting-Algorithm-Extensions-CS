using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace SortingAlgorithmExtensions
{
    //SortingAlgorithmExtensions By Thomas Devoogdt

    public static class HeapSort
    {
        public static void Swap<T>(List<T> arr, int x, int y)   //function to swap elements
        {
            T temp = arr[x];
            arr[x] = arr[y];
            arr[y] = temp;
        }
        public static List<T> BuildHeap<T, TKey>(List<T> arr, ref int heapSize, bool MaxTrMinFa, Func<T, TKey> select) where TKey : IComparable<TKey>
        {
            heapSize = arr.Count - 1;
            if (MaxTrMinFa)
            {
                for (int i = heapSize / 2; i >= 0; i--)
                    Heapify(arr, i, ref heapSize, MaxTrMinFa, select);
            }
            else
            {
                for (int i = 0; i <= heapSize / 2; i++)
                    Heapify(arr, i, ref heapSize, MaxTrMinFa, select);
            }
            return arr;
        }
        public static void Heapify<T, TKey>(List<T> arr, int index, ref int heapSize, bool MaxTrMinFa, Func<T, TKey> select) where TKey : IComparable<TKey>
        {
            int toSwap = index;
            while (true)
            {
                int left = 2 * index + 1;
                int right = 2 * index + 2;

                if (MaxTrMinFa)
                {
                    if (left <= heapSize && select.Invoke(arr[left]).CompareTo(select.Invoke(arr[index])) > 0) toSwap = left;
                    if (right <= heapSize && select.Invoke(arr[right]).CompareTo(select.Invoke(arr[toSwap])) > 0) toSwap = right;
                }
                else
                {
                    if (left <= heapSize && select.Invoke(arr[left]).CompareTo(select.Invoke(arr[index])) < 0) toSwap = left;
                    if (right <= heapSize && select.Invoke(arr[right]).CompareTo(select.Invoke(arr[toSwap])) < 0) toSwap = right;
                }

                if (toSwap != index)
                {
                    Swap(arr, index, toSwap);
                    index = toSwap;
                }
                else return;
            }
        }
        public static List<T> OrderByHeapSort<T, TKey>(this List<T> arr, Func<T, TKey> select, bool MaxTrMinFa = true) where TKey : IComparable<TKey>
        {
            int heapSize = 0;
            BuildHeap(arr, ref heapSize, MaxTrMinFa, select);
            for (int i = arr.Count - 1; i >= 0; i--)
            {
                Swap(arr, 0, i);
                heapSize--;
                Heapify(arr, 0, ref heapSize, MaxTrMinFa, select);
            }
            return arr;
        }
        public static List<T> OrderByHeapSortDescending<T, TKey>(this List<T> arr, Func<T, TKey> select) where TKey : IComparable<TKey>
        {
            return arr.OrderByHeapSort(select, false);
        }
    }

    public static class MedianHeapSort
    {
        public static List<T> OrderByMedianHeapSort<T, TKey>(this List<T> arr, Func<T, TKey> select) where TKey : IComparable<TKey>
        {
            List<T> outputList = new List<T>();
            Heap<T, TKey> minHeap = new Heap<T, TKey>(select, HeapSetting.min);
            Heap<T, TKey> maxHeap = new Heap<T, TKey>(select, HeapSetting.max);
            T mean = arr.First();

            for (int i = 0; i < arr.Count; ++i)
            {
                if (select(mean).CompareTo(select(arr[i])) > 0)  maxHeap.Add(arr[i]); 
                else minHeap.Add(arr[i]); 

                if (maxHeap.lenght - minHeap.lenght > 1)
                {
                    minHeap.Add(maxHeap.DeleteRoot());
                    mean = minHeap.GetRoot();
                }
                else if (maxHeap.lenght - minHeap.lenght < 0)
                {
                    maxHeap.Add(minHeap.DeleteRoot());
                    mean = maxHeap.GetRoot();
                }
            }

            for (int i = 0; i < arr.Count; ++i)
            {
                if (outputList.Count != 0 && select(outputList.First()).CompareTo(select(maxHeap.GetRoot())) <= 0)
                {
                    outputList.Insert(0, maxHeap.DeleteRoot());
                }
                else
                {
                    outputList.Add(maxHeap.DeleteRoot());
                }

                if (maxHeap.lenght < minHeap.lenght && minHeap.lenght != 0)
                {
                    maxHeap.Add(minHeap.DeleteRoot());
                }
            }

            return outputList;
        }
        public static List<T> OrderByMedianHeapSortDescending<T, TKey>(this List<T> arr, Func<T, TKey> select) where TKey : IComparable<TKey>
        {
            arr = arr.OrderByMedianHeapSort(select);
            arr.Reverse();
            return arr;
        }
        public class Heap<T, TKey> where TKey : IComparable<TKey>
        {
            public Heap(Func<T, TKey> select, HeapSetting setting)
            {
                this.select = select;
                this.setting = setting;
            }

            public T GetRoot() { return elements.First(); }
            public void Add(T item)
            {
                elements.Add(item);
                UpPercolate();
            }
            public T DeleteRoot()
            {
                if (elements.Count == 0) return default(T);
                int last = elements.Count - 1;
                T root = elements[0];
                elements[0] = elements[last];
                elements.RemoveAt(last);
                DownPercolate();
                return root;
            }

            private void Swap(int x, int y)
            {
                T temp = elements[x];
                elements[x] = elements[y];
                elements[y] = temp;
            }
            private bool compare(int x, int y)
            {
                return ((setting == HeapSetting.max) ? 1 : -1) * select(elements[x]).CompareTo(select(elements[y])) < 0;
            }
            private void UpPercolate()
            {
                int child = elements.Count;
                int parent = child / 2;

                while (parent != 0 && compare(parent - 1, child - 1))
                {
                    Swap(parent - 1, child - 1);
                    child = parent;
                    parent /= 2;
                }
            }
            private void DownPercolate()
            {
                int parent = 0;
                int child = 0;
                int posChild1 = 1;
                int posChild2 = 2;

                if (elements.Count == 2)
                {
                    posChild1 = 0;
                    posChild2 = 1;
                }

                if (posChild2 <= (elements.Count - 1))
                {
                    child = compare(posChild2, posChild1) ? posChild1 : posChild2;
                }

                while (posChild2 <= (elements.Count - 1) && compare(parent, child))
                {
                    Swap(parent, child);
                    parent = child;
                    posChild1 = parent * 2 + 1;
                    posChild2 = parent * 2 + 2;

                    if (posChild2 <= (elements.Count - 1))
                    {
                        child = compare(posChild2, posChild1) ? posChild1 : posChild2;
                    }
                    else return;
                }
            }

            private List<T> elements = new List<T>();
            public int lenght { get { return elements.Count; } }
            private Func<T, TKey> select;
            private HeapSetting setting;
        }
        public enum HeapSetting { min, max }
    }

    public static class AVLtreeSort
    {
        /* http://www.superstarcoders.com/blogs/posts/efficient-avl-tree-in-c-sharp.aspx */
        public class AvlTree<TKey, TValue> : IEnumerable<TValue>
        {
            //private IComparer<TKey> _comparer;
            //private AvlNode _root;
            public IComparer<TKey> _comparer_general;
            public IComparer<TKey> _comparer_search;
            public AvlNode _root;

            public AvlTree(IComparer<TKey> comparer)
            {
                _comparer_general = comparer;
                _comparer_search = comparer;
            }

            public AvlTree(IComparer<TKey> Searchcomparer, IComparer<TKey> Generalcomparer)
            {
                _comparer_general = Generalcomparer;
                _comparer_search = Searchcomparer;
            }

            public AvlTree()
                : this(Comparer<TKey>.Default)
            {

            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return new AvlNodeEnumerator(_root);
            }

            public void Clear()
            {
                _root = null;
            }

            public bool Search(TKey key, out TValue value)
            {
                AvlNode node = _root;

                while (node != null)
                {
                    if (_comparer_general.Compare(key, node.Key) < 0)
                    {
                        node = node.Left;
                    }
                    else if (_comparer_general.Compare(key, node.Key) > 0)
                    {
                        node = node.Right;
                    }
                    else
                    {
                        value = node.Value;

                        return true;
                    }
                }

                value = default(TValue);

                return false;
            }
            public bool StringSearch(TKey key, out List<TValue> value)
            {
                AvlNodeEnumerator loop = new AvlNodeEnumerator(_root);
                value = new List<TValue>();
                while (loop.MoveNext())
                {
                    if (_comparer_search.Compare(loop.CurrentKey, key) == 0) value.Add(loop.Current);
                }
                return value.Count > 0 ? true : false;
            }

            public void Insert(TKey key, TValue value)
            {
                if (_root == null)
                {
                    _root = new AvlNode { Key = key, Value = value };
                }
                else
                {
                    AvlNode node = _root;

                    while (node != null)
                    {
                        int compare = _comparer_general.Compare(key, node.Key);

                        if (compare < 0)
                        {
                            AvlNode left = node.Left;

                            if (left == null)
                            {
                                node.Left = new AvlNode { Key = key, Value = value, Parent = node };

                                InsertBalance(node, 1);

                                return;
                            }
                            else
                            {
                                node = left;
                            }
                        }
                        else if (compare > 0)
                        {
                            AvlNode right = node.Right;

                            if (right == null)
                            {
                                node.Right = new AvlNode { Key = key, Value = value, Parent = node };

                                InsertBalance(node, -1);

                                return;
                            }
                            else
                            {
                                node = right;
                            }
                        }
                        else
                        {
                            node.Value = value;

                            return;
                        }
                    }
                }
            }

            private void InsertBalance(AvlNode node, int balance)
            {
                while (node != null)
                {
                    balance = (node.Balance += balance);

                    if (balance == 0)
                    {
                        return;
                    }
                    else if (balance == 2)
                    {
                        if (node.Left.Balance == 1)
                        {
                            RotateRight(node);
                        }
                        else
                        {
                            RotateLeftRight(node);
                        }

                        return;
                    }
                    else if (balance == -2)
                    {
                        if (node.Right.Balance == -1)
                        {
                            RotateLeft(node);
                        }
                        else
                        {
                            RotateRightLeft(node);
                        }

                        return;
                    }

                    AvlNode parent = node.Parent;

                    if (parent != null)
                    {
                        balance = parent.Left == node ? 1 : -1;
                    }

                    node = parent;
                }
            }

            public bool Delete(TKey key)
            {
                AvlNode node = _root;

                while (node != null)
                {
                    if (_comparer_general.Compare(key, node.Key) < 0)
                    {
                        node = node.Left;
                    }
                    else if (_comparer_general.Compare(key, node.Key) > 0)
                    {
                        node = node.Right;
                    }
                    else
                    {
                        AvlNode left = node.Left;
                        AvlNode right = node.Right;

                        if (left == null)
                        {
                            if (right == null)
                            {
                                if (node == _root)
                                {
                                    _root = null;
                                }
                                else
                                {
                                    AvlNode parent = node.Parent;

                                    if (parent.Left == node)
                                    {
                                        parent.Left = null;

                                        DeleteBalance(parent, -1);
                                    }
                                    else
                                    {
                                        parent.Right = null;

                                        DeleteBalance(parent, 1);
                                    }
                                }
                            }
                            else
                            {
                                Replace(node, right);

                                DeleteBalance(node, 0);
                            }
                        }
                        else if (right == null)
                        {
                            Replace(node, left);

                            DeleteBalance(node, 0);
                        }
                        else
                        {
                            AvlNode successor = right;

                            if (successor.Left == null)
                            {
                                AvlNode parent = node.Parent;

                                successor.Parent = parent;
                                successor.Left = left;
                                successor.Balance = node.Balance;

                                if (left != null)
                                {
                                    left.Parent = successor;
                                }

                                if (node == _root)
                                {
                                    _root = successor;
                                }
                                else
                                {
                                    if (parent.Left == node)
                                    {
                                        parent.Left = successor;
                                    }
                                    else
                                    {
                                        parent.Right = successor;
                                    }
                                }

                                DeleteBalance(successor, 1);
                            }
                            else
                            {
                                while (successor.Left != null)
                                {
                                    successor = successor.Left;
                                }

                                AvlNode parent = node.Parent;
                                AvlNode successorParent = successor.Parent;
                                AvlNode successorRight = successor.Right;

                                if (successorParent.Left == successor)
                                {
                                    successorParent.Left = successorRight;
                                }
                                else
                                {
                                    successorParent.Right = successorRight;
                                }

                                if (successorRight != null)
                                {
                                    successorRight.Parent = successorParent;
                                }

                                successor.Parent = parent;
                                successor.Left = left;
                                successor.Balance = node.Balance;
                                successor.Right = right;
                                right.Parent = successor;

                                if (left != null)
                                {
                                    left.Parent = successor;
                                }

                                if (node == _root)
                                {
                                    _root = successor;
                                }
                                else
                                {
                                    if (parent.Left == node)
                                    {
                                        parent.Left = successor;
                                    }
                                    else
                                    {
                                        parent.Right = successor;
                                    }
                                }

                                DeleteBalance(successorParent, -1);
                            }
                        }

                        return true;
                    }
                }

                return false;
            }

            private void DeleteBalance(AvlNode node, int balance)
            {
                while (node != null)
                {
                    balance = (node.Balance += balance);

                    if (balance == 2)
                    {
                        if (node.Left.Balance >= 0)
                        {
                            node = RotateRight(node);

                            if (node.Balance == -1)
                            {
                                return;
                            }
                        }
                        else
                        {
                            node = RotateLeftRight(node);
                        }
                    }
                    else if (balance == -2)
                    {
                        if (node.Right.Balance <= 0)
                        {
                            node = RotateLeft(node);

                            if (node.Balance == 1)
                            {
                                return;
                            }
                        }
                        else
                        {
                            node = RotateRightLeft(node);
                        }
                    }
                    else if (balance != 0)
                    {
                        return;
                    }

                    AvlNode parent = node.Parent;

                    if (parent != null)
                    {
                        balance = parent.Left == node ? -1 : 1;
                    }

                    node = parent;
                }
            }

            private AvlNode RotateLeft(AvlNode node)
            {
                AvlNode right = node.Right;
                AvlNode rightLeft = right.Left;
                AvlNode parent = node.Parent;

                right.Parent = parent;
                right.Left = node;
                node.Right = rightLeft;
                node.Parent = right;

                if (rightLeft != null)
                {
                    rightLeft.Parent = node;
                }

                if (node == _root)
                {
                    _root = right;
                }
                else if (parent.Right == node)
                {
                    parent.Right = right;
                }
                else
                {
                    parent.Left = right;
                }

                right.Balance++;
                node.Balance = -right.Balance;

                return right;
            }

            private AvlNode RotateRight(AvlNode node)
            {
                AvlNode left = node.Left;
                AvlNode leftRight = left.Right;
                AvlNode parent = node.Parent;

                left.Parent = parent;
                left.Right = node;
                node.Left = leftRight;
                node.Parent = left;

                if (leftRight != null)
                {
                    leftRight.Parent = node;
                }

                if (node == _root)
                {
                    _root = left;
                }
                else if (parent.Left == node)
                {
                    parent.Left = left;
                }
                else
                {
                    parent.Right = left;
                }

                left.Balance--;
                node.Balance = -left.Balance;

                return left;
            }

            private AvlNode RotateLeftRight(AvlNode node)
            {
                AvlNode left = node.Left;
                AvlNode leftRight = left.Right;
                AvlNode parent = node.Parent;
                AvlNode leftRightRight = leftRight.Right;
                AvlNode leftRightLeft = leftRight.Left;

                leftRight.Parent = parent;
                node.Left = leftRightRight;
                left.Right = leftRightLeft;
                leftRight.Left = left;
                leftRight.Right = node;
                left.Parent = leftRight;
                node.Parent = leftRight;

                if (leftRightRight != null)
                {
                    leftRightRight.Parent = node;
                }

                if (leftRightLeft != null)
                {
                    leftRightLeft.Parent = left;
                }

                if (node == _root)
                {
                    _root = leftRight;
                }
                else if (parent.Left == node)
                {
                    parent.Left = leftRight;
                }
                else
                {
                    parent.Right = leftRight;
                }

                if (leftRight.Balance == -1)
                {
                    node.Balance = 0;
                    left.Balance = 1;
                }
                else if (leftRight.Balance == 0)
                {
                    node.Balance = 0;
                    left.Balance = 0;
                }
                else
                {
                    node.Balance = -1;
                    left.Balance = 0;
                }

                leftRight.Balance = 0;

                return leftRight;
            }

            private AvlNode RotateRightLeft(AvlNode node)
            {
                AvlNode right = node.Right;
                AvlNode rightLeft = right.Left;
                AvlNode parent = node.Parent;
                AvlNode rightLeftLeft = rightLeft.Left;
                AvlNode rightLeftRight = rightLeft.Right;

                rightLeft.Parent = parent;
                node.Right = rightLeftLeft;
                right.Left = rightLeftRight;
                rightLeft.Right = right;
                rightLeft.Left = node;
                right.Parent = rightLeft;
                node.Parent = rightLeft;

                if (rightLeftLeft != null)
                {
                    rightLeftLeft.Parent = node;
                }

                if (rightLeftRight != null)
                {
                    rightLeftRight.Parent = right;
                }

                if (node == _root)
                {
                    _root = rightLeft;
                }
                else if (parent.Right == node)
                {
                    parent.Right = rightLeft;
                }
                else
                {
                    parent.Left = rightLeft;
                }

                if (rightLeft.Balance == 1)
                {
                    node.Balance = 0;
                    right.Balance = -1;
                }
                else if (rightLeft.Balance == 0)
                {
                    node.Balance = 0;
                    right.Balance = 0;
                }
                else
                {
                    node.Balance = 1;
                    right.Balance = 0;
                }

                rightLeft.Balance = 0;

                return rightLeft;
            }

            private static void Replace(AvlNode target, AvlNode source)
            {
                AvlNode left = source.Left;
                AvlNode right = source.Right;

                target.Balance = source.Balance;
                target.Key = source.Key;
                target.Value = source.Value;
                target.Left = left;
                target.Right = right;

                if (left != null)
                {
                    left.Parent = target;
                }

                if (right != null)
                {
                    right.Parent = target;
                }
            }

            public class AvlNode
            {
                public AvlNode Parent;
                public AvlNode Left;
                public AvlNode Right;
                public TKey Key;
                public TValue Value;
                public int Balance;
            }

            public class AvlNodeEnumerator : IEnumerator<TValue>
            {
                private AvlNode _root;
                private Action _action;
                private AvlNode _current;
                private AvlNode _right;

                public AvlNodeEnumerator(AvlNode root)
                {
                    _right = _root = root;

                    _action = root == null ? Action.End : Action.Right;
                }

                public bool MoveNext()
                {
                    switch (_action)
                    {
                        case Action.Right:
                            _current = _right;

                            while (_current.Left != null)
                            {
                                _current = _current.Left;
                            }

                            _right = _current.Right;

                            _action = _right != null ? Action.Right : Action.Parent;

                            return true;
                        case Action.Parent:
                            while (_current.Parent != null)
                            {
                                AvlNode previous = _current;

                                _current = _current.Parent;

                                if (_current.Left == previous)
                                {
                                    _right = _current.Right;

                                    _action = _right != null ? Action.Right : Action.Parent;

                                    return true;
                                }
                            }

                            _action = Action.End;

                            return false;
                        default:
                            return false;
                    }
                }

                public void Reset()
                {
                    _right = _root;

                    _action = _root == null ? Action.End : Action.Right;
                }

                public TValue Current
                {
                    get
                    {
                        return _current.Value;
                    }
                }

                public TKey CurrentKey
                {
                    get
                    {
                        return _current.Key;
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return Current;
                    }
                }

                public void Dispose()
                {

                }

                enum Action
                {
                    Parent,
                    Right,
                    End
                }
            }
        }
        public static List<T> OrderByAVLtreeSort<T, TKey>(this List<T> arr, Func<T, TKey> select, bool MaxTrMinFa = true) where TKey : IComparable<TKey>
        {
            AvlTree<TKey, T> avlTree = new AvlTree<TKey, T>();
            foreach (T item in arr)
            {
                avlTree.Insert(select.Invoke(item), item);
            }
            arr = avlTree.ToList();
            if (!MaxTrMinFa) arr.Reverse();
            return arr;
        }
        public static List<T> OrderByAVLtreeSortDescending<T, TKey>(this List<T> arr, Func<T, TKey> select) where TKey : IComparable<TKey>
        {
            return arr.OrderByAVLtreeSort(select, false);
        }
    }

    public static class SelectionSort
    {
        public static void Swap<T>(List<T> arr, int x, int y)   //function to swap elements
        {
            T temp = arr[x];
            arr[x] = arr[y];
            arr[y] = temp;
        }
        public static List<T> OrderBySelectionSort<T, TKey>(this List<T> arr, Func<T, TKey> select, bool MaxTrMinFa = true) where TKey : IComparable<TKey>
        {
            for (int i = arr.Count - 1, maxIndex = 0; i > 0; i--, maxIndex = 0)
            {
                for (int j = 1; j <= i; j++)
                {
                    if (MaxTrMinFa && select.Invoke(arr[j]).CompareTo(select.Invoke(arr[maxIndex])) > 0) maxIndex = j;
                    else if (!MaxTrMinFa && select.Invoke(arr[j]).CompareTo(select.Invoke(arr[maxIndex])) < 0) maxIndex = j;
                }
                Swap(arr, maxIndex, i);
            }
            return arr;
        }
        public static List<T> OrderBySelectionSortDescending<T, TKey>(this List<T> arr, Func<T, TKey> select) where TKey : IComparable<TKey>
        {
            return arr.OrderBySelectionSort(select, false);
        }
    }

    public static class BubbleSort
    {
        public static void Swap<T>(List<T> arr, int x, int y)   //function to swap elements
        {
            T temp = arr[x];
            arr[x] = arr[y];
            arr[y] = temp;
        }
        public static List<T> OrderByBubbleSort<T, TKey>(this List<T> arr, Func<T, TKey> select, bool MaxTrMinFa = true) where TKey : IComparable<TKey>
        {
            for (int write = 0; write < arr.Count; write++)
            {
                bool doBreak = true;
                for (int sort = 0; sort < arr.Count - 1; sort++)
                {
                    if (MaxTrMinFa && select.Invoke(arr[sort]).CompareTo(select.Invoke(arr[sort + 1])) > 0)
                    {
                        Swap(arr, sort + 1, sort);
                        doBreak = false;
                    }
                    else if (!MaxTrMinFa && select.Invoke(arr[sort]).CompareTo(select.Invoke(arr[sort + 1])) < 0)
                    {
                        Swap(arr, sort + 1, sort);
                        doBreak = false;
                    }
                }
                if (doBreak) { return arr; }
            }
            return arr;
        }
        public static List<T> OrderByBubbleSortDescending<T, TKey>(this List<T> arr, Func<T, TKey> select) where TKey : IComparable<TKey>
        {
            return arr.OrderByBubbleSort(select, false);
        }
    }

    public static class InsertionSort
    {
        public static List<T> OrderByInsertionSort<T, TKey>(this List<T> arr, Func<T, TKey> select, bool MaxTrMinFa = true) where TKey : IComparable<TKey>
        {
            for (int i = 1; i < arr.Count; i++)
            {
                T temp = arr[i];
                int j;
                for (j = i - 1; j >= 0 && (MaxTrMinFa ? (select.Invoke(arr[j]).CompareTo(select.Invoke(temp)) > 0) : (select.Invoke(arr[j]).CompareTo(select.Invoke(temp)) < 0)); j--)
                {
                    arr[j + 1] = arr[j];
                }
                arr[j + 1] = temp;
            }
            return arr;
        }
        public static List<T> OrderByInsertionSortDescending<T, TKey>(this List<T> arr, Func<T, TKey> select) where TKey : IComparable<TKey>
        {
            return arr.OrderByInsertionSort(select, false);
        }
    }
}