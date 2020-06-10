using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.IO;
using System.Linq;
using System;
using UnityEditor;
using Assets.Src.Extensions;

namespace Assets.Src.EditorExtensions
{
    public class FileTreeView : TreeView
    {
        private string _rootFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assets\\Art Assets\\");

        private int _amountOfTreeItems = 0;
        private TreeViewItem _filledRoot;
        private IEnumerable<TreeViewItem> _flattenedItems;

        public FileTreeView(TreeViewState treeViewState) : base(treeViewState)
        {
            Reload();

            multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(BuildColumns()));
            multiColumnHeader.ResizeToFit();

        }

        protected override TreeViewItem BuildRoot()
        {
            _filledRoot = new TreeViewItem(id: _amountOfTreeItems, depth: -1, displayName: "Assets");
            var rootFolder = new DirectoryInfo(_rootFolder);

            _amountOfTreeItems++;
            RecursionTreeViewGenerator(_filledRoot, rootFolder);

            SetupDepthsFromParentsAndChildren(_filledRoot);

            _flattenedItems = _flattenList(_filledRoot);

            return _filledRoot;
        }

        protected override void ContextClickedItem(int id)
        {
            //base.ContextClickedItem(id);

            
            
            var item = _flattenedItems.Cast<FileTreeViewElement>().FirstOrDefault(c => c.id == id);

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Download File"), false, new GenericMenu.MenuFunction2(DownloadFile), item);
            menu.AddItem(new GUIContent("Delete file"), false, new GenericMenu.MenuFunction2(DeleteFile), item);
            menu.ShowAsContext();

        }

        void DeleteFile(object obj)
        {
            var fileTreeViewElement = obj as FileTreeViewElement;

            Debug.Log(fileTreeViewElement.RelativePath);
        }

        void DownloadFile(object obj)
        {
            var fileTreeViewElement = obj as FileTreeViewElement;
            Debug.Log(fileTreeViewElement.RelativePath);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), args.item as FileTreeViewElement, args.GetColumn(i), ref args);
            }
        }


        void CellGUI(Rect cellRect, FileTreeViewElement item, int columnIndex, ref RowGUIArgs args)
        {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight(ref cellRect);

            switch (columnIndex)
            {
                case 0:
                    {
                        base.RowGUI(args);
                    }
                    break;
                case 1:
                    {
                        EditorGUI.LabelField(cellRect, new GUIContent(item.RelativePath));
                    }
                    break;

                case 2:
                    {
                        if (!item.IsFolder)
                        {
                            Rect toggleRect = cellRect;

                            EditorGUI.Toggle(toggleRect, new GUIContent(item.Synced ? "Synced" : "Out-of-Sync"), item.Synced);
                        }
                    }
                    break;
            }
        }



        private void RecursionTreeViewGenerator(TreeViewItem currentItem, DirectoryInfo currentPosition, int depth = 0)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;

            try
            {
                files = currentPosition.GetFiles("*.*");
            }
            catch (UnauthorizedAccessException e)
            {
                Debug.LogError(e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                Debug.LogError(e.Message);
            }

            // Now find all the subdirectories under this directory.
            subDirs = currentPosition.GetDirectories();

            foreach (DirectoryInfo dirInfo in subDirs)
            {
                var relativePath = dirInfo.FullName.Replace(_rootFolder, "");

                var folderRoot = new FileTreeViewElement(id: _getNewIdNumber(), depth: depth, displayName: Path.GetFileNameWithoutExtension(relativePath));
                folderRoot.RelativePath = relativePath;
                folderRoot.IsFolder = true;
                folderRoot.Synced = false;

                currentItem.AddChild(folderRoot);
                // Resursive call for each subdirectory.
                RecursionTreeViewGenerator(folderRoot, dirInfo, depth++);
            }

            if (files != null)
            {
                foreach (var fi in files)
                {
                    var relativePath = fi.FullName.Replace(_rootFolder, "");
                    if (!relativePath.EndsWith(".meta"))
                    {
                        var treeViewItem = new FileTreeViewElement(id: _getNewIdNumber(), depth: depth, displayName: Path.GetFileNameWithoutExtension(relativePath))
                        {
                            RelativePath = relativePath,
                            Synced = true,
                            IsFolder = false
                        };
                        currentItem.AddChild(treeViewItem);
                    }
                }
            }
        }


        static MultiColumnHeaderState.Column[] BuildColumns()
        {
            return new List<MultiColumnHeaderState.Column>
                {
                    new MultiColumnHeaderState.Column
                    {
                        autoResize = true,
                        headerContent = new GUIContent("FileName")
                    },
                    new MultiColumnHeaderState.Column
                    {
                        autoResize = true,
                        headerContent = new GUIContent("FilePath")
                    },
                    new MultiColumnHeaderState.Column
                    {
                        autoResize = true,
                        headerContent = new GUIContent("Resync")
                    }
                }.ToArray();
        }

        private int _getNewIdNumber()
        {
            _amountOfTreeItems += 1;
            return _amountOfTreeItems;
        }

        private IEnumerable<TreeViewItem> _flattenList(TreeViewItem root)
        {
            return root.children.Flatten(c =>
            {
                if (!c.hasChildren)
                    return Enumerable.Empty<TreeViewItem>();

                return c.children;
            });
        }
    }
}