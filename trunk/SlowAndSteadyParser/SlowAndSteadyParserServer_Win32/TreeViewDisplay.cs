using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SlowAndSteadyParser
{
    public class TreeViewDisplay
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private TreeView m_treeview = null;        
        private TreeNode m_root = null;

        #region Initial

        private void InitialAllNode()
        {
            //clear
            m_treeview.Nodes.Clear();
            //root
            m_root = new TreeNode("服务器");
            m_root.ImageKey = "serveronline.png";
            m_root.SelectedImageKey = "serveronline.png";

            //domain
            SortedDictionary<string, Domain> sortedbyname = new SortedDictionary<string, Domain>();
            foreach (Domain d in DomainManager.DomainList.Values)
            {
                sortedbyname.Add(d.Name, d);                
            }
            foreach (Domain d in sortedbyname.Values)
                TreeViewAddDomain(d);
            
            //加入treeview
            m_treeview.Nodes.AddRange(new TreeNode[] { m_root });

            RefreshAllDomainNode();
        }

        public void RefreshAllDomainNode()
        {
            m_root.Expand();
            foreach (TreeNode tn in m_root.Nodes)
            {
                SetDomainNodeStatus(tn);                
            }
        }

        public void SetServerNodeStatus()
        {
            string servermode;
            if (ServerControlManager.IsServerModeRemote)
                servermode = " - 服务器模式";
            else
                servermode = " - 单机模式";

            if (ServerControlManager.IsServerRunning)
            {
                m_root.Text = "服务器 [运行中]" + servermode;
                m_root.ImageKey = "serveronline.png";
                m_root.SelectedImageKey = "serveronline.png";
            }
            else
            {
                m_root.Text = "服务器 [停止]" + servermode;
                m_root.ImageKey = "serveroffline.png";
                m_root.SelectedImageKey = "serveroffline.png";
            }
        }

        public void SetDomainNodeStatus(TreeNode tn)
        {
            if (tn != null && tn.Tag.GetType().Name == "Domain")
            {
                Domain d = tn.Tag as Domain;
                if (d.Enable)
                {
                    tn.Text = "[启用]解决方案 " + d.Name;
                    tn.ImageKey = "domainonline.png";
                    tn.SelectedImageKey = "domainonline.png";
                }
                else
                {
                    tn.Text = "[禁用]解决方案 " + d.Name;
                    tn.ImageKey = "domainoffline.png";
                    tn.SelectedImageKey = "domainoffline.png";
                }
            }
            else
                log.Error("非Domain节点被传递到SetDomainNodeStatus方法");
        }

        public TreeNode TreeViewAddDomain(Domain d)
        {
            //生成节点
            TreeNode treeNodePreparationScript = new TreeNode("数据初始化脚本");
            treeNodePreparationScript.ImageKey = "script.png";
            treeNodePreparationScript.SelectedImageKey = "script.png";
            treeNodePreparationScript.Tag = "PreparationScript";
            //treeNodePreparationScript.Name = d."PreparationScriptTreeNode";

            TreeNode treeNodeParserScript = new TreeNode("数据采集脚本");
            treeNodeParserScript.ImageKey = "script.png";
            treeNodeParserScript.SelectedImageKey = "script.png";
            treeNodeParserScript.Tag = "ParserScript";
            //treeNodeParserScript.Name = "ParserScriptTreeNode";

            TreeNode treeNodeStorageScript = new TreeNode("数据入库脚本");
            treeNodeStorageScript.ImageKey = "script.png";
            treeNodeStorageScript.SelectedImageKey = "script.png";
            treeNodeStorageScript.Tag = "StorageScript";
            //treeNodeStorageScript.Name = "StorageScriptTreeNode";

            TreeNode treeNodeTest = new TreeNode("测试");
            treeNodeTest.ImageKey = "testcenter.png";
            treeNodeTest.SelectedImageKey = "testcenter.png";
            treeNodeTest.Tag = "Test";

            TreeNode treeNodeDomainLog = new TreeNode("日志");
            treeNodeDomainLog.ImageKey = "dblog.png";
            treeNodeDomainLog.SelectedImageKey = "dblog.png";
            treeNodeDomainLog.Tag = "Log";
            //treeNodeDomainLog.Name = "DomainLogTreeNode";

            TreeNode treeNodeDomain = new TreeNode("解决方案 " + d.Name, new TreeNode[]{
                        treeNodePreparationScript,
                        treeNodeParserScript,
                        treeNodeStorageScript,
                        treeNodeTest,
                        treeNodeDomainLog});
            treeNodeDomain.Tag = d;
            SetDomainNodeStatus(treeNodeDomain);

            //加入根目录
            m_root.Nodes.Add(treeNodeDomain);
            return treeNodeDomain;
        }

        public TreeNode TreeViewAddDomain(Domain d, bool IsSelected, bool IsExpanded)
        {
            TreeNode t = TreeViewAddDomain(d);
            if (IsSelected)
                m_treeview.SelectedNode = t;
            if (IsExpanded)
                t.Expand();
            else
                t.Collapse();
            return t;
        }

        public void TreeViewRemoveDomainNode(TreeNode tn)
        {
            if (FocusdNodeType == TreeViewFocus.Domain)
            {
                m_root.Nodes.Remove(tn);
            }
        }

        public TreeViewFocus TreeNodeGetType(TreeNode tn)
        {
            if (tn == null)
                return TreeViewFocus.None;
            if (tn == m_root)
                return TreeViewFocus.Server;
            if (tn.Tag.GetType().Name == "Domain")
                return TreeViewFocus.Domain;
            switch (tn.Tag as string)
            {
                case "PreparationScript":
                    return TreeViewFocus.Script_Preparation;
                case "ParserScript":
                    return TreeViewFocus.Script_Parser;
                case "StorageScript":
                    return TreeViewFocus.Script_Storage;
                case "Log":
                    return TreeViewFocus.Log;
                case "Test":
                    return TreeViewFocus.Test;
            }
            return TreeViewFocus.None;
        }

        public Domain TreeNodeGetDomain(TreeNode tn)
        {
            if (TreeNodeGetType(tn) == TreeViewFocus.Domain)
                return tn.Tag as Domain;
            return null;
        }

        public void MoveToRoot()
        {
            m_treeview.SelectedNode = m_root;
        }

        #endregion

        #region Focus Methods

        public TreeViewFocus FocusdNodeType
        {
            get
            {
                return TreeNodeGetType(m_treeview.SelectedNode);
            }
        }

        public Domain TreeViewRemoveFocusdDomainNode()
        {
            Domain d = TreeViewGetFocusdDomain();
            TreeViewRemoveDomainNode(m_treeview.SelectedNode);
            return d;
        }

        public Domain TreeViewGetFocusdDomain()
        {
            return TreeNodeGetDomain(m_treeview.SelectedNode);
        }
        #endregion

        #region 静态函数

        



        #endregion

        #region 构造函数

        public TreeViewDisplay(TreeView thistreeview)
        {
            m_treeview = thistreeview;
            InitialAllNode();
        }

        #endregion
    }

    public enum TreeViewFocus
    {
        None,
        Server,
        Domain,
        Script_Preparation,
        Script_Parser,
        Script_Storage,
        Test,
        Log
    }
}
