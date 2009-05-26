// ***************************************************************
//  RobotRulesParser   version:  1.0   Date: 12/15/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Diagnostics;
using System.Web;

using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Support;

namespace Winista.Text.HtmlParser.Http
{
	/// <summary>
	/// Summary description for RobotRulesParser.
	/// </summary>
	public class RobotRulesParser
	{
		/// <summary> This class holds the rules which were parsed from a robots.txt
		/// file, and can test paths against those rules.
		/// </summary>
		public class RobotRuleSet
		{
			//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1199_3"'
			/// <summary> Get expire time</summary>
			/// <summary> Change when the ruleset goes stale.</summary>
			virtual public long ExpireTime
			{
				get
				{
					return m_expireTime;
				}
				
				set
				{
					this.m_expireTime = value;
				}
				
			}
			internal System.Collections.ArrayList m_tmpEntries;
			internal RobotsEntry[] m_entries;
			internal long m_expireTime;
			
			internal class RobotsEntry
			{
				private void  InitBlock(RobotRuleSet enclosingInstance)
				{
					this.m_enclosingInstance = enclosingInstance;
				}
				private RobotRuleSet m_enclosingInstance;
				public RobotRuleSet Enclosing_Instance
				{
					get
					{
						return m_enclosingInstance;
					}
					
				}
				internal System.String m_strPrefix;
				internal bool m_bAllowed;
				
				internal RobotsEntry(RobotRuleSet enclosingInstance, System.String prefix, bool allowed)
				{
					InitBlock(enclosingInstance);
					this.m_strPrefix = prefix;
					this.m_bAllowed = allowed;
				}
			}
			

			/// <summary> should not be instantiated from outside RobotRulesParser</summary>
			internal RobotRuleSet()
			{
				m_tmpEntries = new System.Collections.ArrayList();
				m_entries = null;
			}
			
			
			internal void AddPrefix(System.String prefix, bool allow)
			{
				if (m_tmpEntries == null)
				{
					m_tmpEntries = new System.Collections.ArrayList();
					if (m_entries != null)
					{
						for (int i = 0; i < m_entries.Length; i++)
						{
							m_tmpEntries.Add(m_entries[i]);
						}
					}
					m_entries = null;
				}
				
				m_tmpEntries.Add(new RobotsEntry(this, prefix, allow));
			}
			
			
			internal void ClearPrefixes()
			{
				if (m_tmpEntries == null)
				{
					m_tmpEntries = new System.Collections.ArrayList();
					m_entries = null;
				}
				else
				{
					m_tmpEntries.Clear();
				}
			}
			
			/// <summary>  Returns <code>false</code> if the <code>robots.txt</code> file
			/// prohibits us from accessing the given <code>path</code>, or
			/// <code>true</code> otherwise.
			/// </summary>
			public virtual bool IsAllowed(System.String path)
			{
				try
				{
					path = HttpUtility.UrlDecode(path, System.Text.Encoding.GetEncoding(RobotRulesParser.CHARACTER_ENCODING.ToLower()));
				}
				catch (System.Exception e)
				{
					// just ignore it- we can still try to match 
					// path prefixes
				}
				
				if (m_entries == null)
				{
					m_entries = new RobotsEntry[m_tmpEntries.Count];
					m_entries = (RobotsEntry[]) Support.ICollectionSupport.ToArray(m_tmpEntries, m_entries);
					m_tmpEntries = null;
				}
				
				int pos = 0;
				int end = m_entries.Length;
				while (pos < end)
				{
					if (path.StartsWith(m_entries[pos].m_strPrefix))
						return m_entries[pos].m_bAllowed;
					pos++;
				}
				
				return true;
			}
			
			
			public override System.String ToString()
			{
				IsAllowed("x"); // force String[] representation
				System.Text.StringBuilder buf = new System.Text.StringBuilder();
				for (int i = 0; i < m_entries.Length; i++)
					if (m_entries[i].m_bAllowed)
						buf.Append("Allow: " + m_entries[i].m_strPrefix + System.Environment.NewLine);
					else
						buf.Append("Disallow: " + m_entries[i].m_strPrefix + System.Environment.NewLine);
				return buf.ToString();
			}
		}
		
		private static readonly bool ALLOW_FORBIDDEN = ParserConf.GetConfiguration().GetBoolean("http.robots.403.allow", false);
		
		private static readonly System.String[] AGENTS = Agents;

		private static readonly System.Collections.Hashtable CACHE = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
		
		private const System.String CHARACTER_ENCODING = "UTF-8";

		private static readonly int NO_PRECEDENCE = System.Int32.MaxValue;
		
		private static readonly RobotRuleSet EMPTY_RULES = new RobotRuleSet();
		
		private static RobotRuleSet FORBID_ALL_RULES;
		
		private System.Collections.Hashtable m_robotNames;

		private const int BUFSIZE = 2048;

		public RobotRulesParser():this(AGENTS)
		{
		}

		/// <summary>  Creates a new <code>RobotRulesParser</code> which will use the
		/// supplied <code>robotNames</code> when choosing which stanza to
		/// follow in <code>robots.txt</code> files.  Any name in the array
		/// may be matched.  The order of the <code>robotNames</code>
		/// determines the precedence- if many names are matched, only the
		/// rules associated with the robot name having the smallest index
		/// will be used.
		/// </summary>
		public RobotRulesParser(System.String[] robotNames)
		{
			//UPGRADE_TODO: Class 'java.util.HashMap' was converted to 'System.Collections.Hashtable' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilHashMap_3"'
			this.m_robotNames = new System.Collections.Hashtable();
			for (int i = 0; i < robotNames.Length; i++)
			{
				this.m_robotNames[robotNames[i].ToLower()] = (System.Int32) i;
			}
			// always make sure "*" is included
			if (!this.m_robotNames.ContainsKey("*"))
				this.m_robotNames["*"] = (System.Int32) robotNames.Length;
		}

		private static System.String[] Agents
		{
			get
			{
				//
				// Grab the agent names we advertise to robots files.
				//
				System.String agentName = ParserConf.GetConfiguration().GetPoperty("http.agent.name");
				System.String agentNames = ParserConf.GetConfiguration().GetPoperty("http.robots.agents");
				Tokenizer tok = new Tokenizer(agentNames, ",");
				System.Collections.ArrayList agents = new System.Collections.ArrayList();
				while (tok.HasMoreTokens())
				{
					agents.Add(tok.NextToken().Trim());
				}
				
				//
				// If there are no agents for robots-parsing, use our 
				// default agent-string.  If both are present, our agent-string
				// should be the first one we advertise to robots-parsing.
				// 
				if (agents.Count == 0)
				{
					agents.Add(agentName);
					//LOG.severe("No agents listed in 'http.robots.agents' property!");
					Trace.WriteLine("No agents listed in 'http.robots.agents' property!");
				}
				else if (!((System.String) agents[0]).ToUpper().Equals(agentName.ToUpper()))
				{
					agents.Insert(0, agentName);
					//LOG.severe("Agent we advertise (" + agentName + ") not listed first in 'http.robots.agents' property!");
					Trace.WriteLine("Agent we advertise (" + agentName + ") not listed first in 'http.robots.agents' property!");
				}
				
				return (System.String[]) ICollectionSupport.ToArray(agents, new System.String[agents.Count]);
			}
			
		}
		/// <summary>  Returns a <code>RobotRuleSet</code> object appropriate for use
		/// when the <code>robots.txt</code> file is empty or missing; all
		/// requests are allowed.
		/// </summary>
		internal static RobotRuleSet EmptyRules
		{
			get
			{
				return EMPTY_RULES;
			}
			
		}
		/// <summary>  Returns a <code>RobotRuleSet</code> object appropriate for use
		/// when the <code>robots.txt</code> file is not fetched due to a
		/// <code>403/Forbidden</code> response; all requests are
		/// disallowed.
		/// </summary>
		internal static RobotRuleSet ForbidAllRules
		{
			get
			{
				RobotRuleSet rules = new RobotRuleSet();
				rules.AddPrefix("", false);
				return rules;
			}
			
		}

		/// <summary> Returns a {@link RobotRuleSet} object which encapsulates the
		/// rules parsed from the supplied <code>robotContent</code>.
		/// </summary>
		internal virtual RobotRuleSet ParseRules(byte[] robotContent)
		{
			if (robotContent == null)
			{
				return EMPTY_RULES;
			}
			
			System.String content = new System.String(SupportMisc.ToCharArray(robotContent));
			
			Tokenizer lineParser = new Tokenizer(content, "\n\r");
			
			RobotRuleSet bestRulesSoFar = null;
			int bestPrecedenceSoFar = NO_PRECEDENCE;
			
			RobotRuleSet currentRules = new RobotRuleSet();
			int currentPrecedence = NO_PRECEDENCE;
			
			bool addRules = false; // in stanza for our robot
			bool doneAgents = false; // detect multiple agent lines
			
			while (lineParser.HasMoreTokens())
			{
				System.String line = lineParser.NextToken();
				
				// trim out comments and whitespace
				int hashPos = line.IndexOf("#");
				if (hashPos >= 0)
					line = line.Substring(0, (hashPos) - (0));
				line = line.Trim();
				
				if ((line.Length >= 11) && (line.Substring(0, (11) - (0)).ToUpper().Equals("User-agent:".ToUpper())))
				{
					
					if (doneAgents)
					{
						if (currentPrecedence < bestPrecedenceSoFar)
						{
							bestPrecedenceSoFar = currentPrecedence;
							bestRulesSoFar = currentRules;
							currentPrecedence = NO_PRECEDENCE;
							currentRules = new RobotRuleSet();
						}
						addRules = false;
					}
					doneAgents = false;
					
					System.String agentNames = line.Substring(line.IndexOf(":") + 1);
					agentNames = agentNames.Trim();
					Tokenizer agentTokenizer = new Tokenizer(agentNames);
					
					while (agentTokenizer.HasMoreTokens())
					{
						// for each agent listed, see if it's us:
						System.String agentName = agentTokenizer.NextToken().ToLower();
						
						object obInt = m_robotNames[agentName];
						
						if (obInt != null)
						{
							int precedence = (Int32)obInt;
							if ((precedence < currentPrecedence) && (precedence < bestPrecedenceSoFar))
								currentPrecedence = precedence;
						}
					}
					
					if (currentPrecedence < bestPrecedenceSoFar)
						addRules = true;
				}
				else if ((line.Length >= 9) && (line.Substring(0, (9) - (0)).ToUpper().Equals("Disallow:".ToUpper())))
				{
					
					doneAgents = true;
					System.String path = line.Substring(line.IndexOf(":") + 1);
					path = path.Trim();
					try
					{
						path = HttpUtility.UrlDecode(path, System.Text.Encoding.GetEncoding(CHARACTER_ENCODING.ToLower()));
					}
					catch (System.Exception e)
					{
						//LOG.warning("error parsing robots rules- can't decode path: " + path);
						Trace.WriteLine("error parsing robots rules- can't decode path: " + path);
					}
					
					if (path.Length == 0)
					{
						// "empty rule"
						if (addRules)
							currentRules.ClearPrefixes();
					}
					else
					{
						// rule with path
						if (addRules)
							currentRules.AddPrefix(path, false);
					}
				}
				else if ((line.Length >= 6) && (line.Substring(0, (6) - (0)).ToUpper().Equals("Allow:".ToUpper())))
				{
					
					doneAgents = true;
					System.String path = line.Substring(line.IndexOf(":") + 1);
					path = path.Trim();
					
					if (path.Length == 0)
					{
						// "empty rule"- treat same as empty disallow
						if (addRules)
							currentRules.ClearPrefixes();
					}
					else
					{
						// rule with path
						if (addRules)
							currentRules.AddPrefix(path, true);
					}
				}
			}
			
			if (currentPrecedence < bestPrecedenceSoFar)
			{
				bestPrecedenceSoFar = currentPrecedence;
				bestRulesSoFar = currentRules;
			}
			
			if (bestPrecedenceSoFar == NO_PRECEDENCE)
				return EMPTY_RULES;
			return bestRulesSoFar;
		}
		
		public static bool IsAllowed(System.Uri url)
		{
			
			System.String host = url.Host;
			
			RobotRuleSet robotRules = (RobotRuleSet) CACHE[host];
			
			if (robotRules == null)
			{
				// cache miss
				//UPGRADE_TODO: Class 'java.net.URL' was converted to a 'System.Uri' which does not throw an exception if a URL specifies an unknown protocol. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1132_3"'
				HttpResponseMgr response = new HttpResponseMgr(new System.Uri(url, "/robots.txt"));
				
				if (response.Code == 200)
					// found rules: parse them
					robotRules = new RobotRulesParser().ParseRules(response.Content);
				else if ((response.Code == 403) && (!ALLOW_FORBIDDEN))
					robotRules = FORBID_ALL_RULES;
					// use forbid all
				else
					robotRules = EMPTY_RULES; // use default rules
				
				CACHE[host] = robotRules; // cache rules for host
			}
			
			System.String path = url.AbsolutePath; // check rules
			if ((path == null) || "".Equals(path))
			{
				path = "/";
			}
			
			return robotRules.IsAllowed(path);
		}
	}
}
