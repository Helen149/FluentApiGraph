using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace FluentApi.Graph
{

	public class DotGraphBuilder
	{
		private Graph graph;

		public DotGraphBuilder(Graph graph)
		{
			this.graph = graph;
		}

		public static DotGraphBuilder DirectedGraph(string graphName)
		{
			return new DotGraphBuilder(new Graph(graphName, true, false));
		}

		public static DotGraphBuilder NondirectedGraph(string graphName)
		{
			return new DotGraphBuilder(new Graph(graphName, false, false));
		}

		public string Build()
		{
			return DotFormatExtensions.ToDotFormat(graph);
		}

		public CreatorNode AddNode(string nameNode)
		{
			graph.AddNode(nameNode);
			return new CreatorNode(this, graph.Nodes.Last().Attributes);
		}

		public CreatorEdge AddEdge(string sourceNode, string destinationNode)
		{
			graph.AddEdge(sourceNode, destinationNode);
			return new CreatorEdge(this, graph.Edges.Last().Attributes);
		}
	}

	public enum NodeShape
	{
		Ellipse,
		Box
	}

	public static class CreatorGraphObjExtension
	{
		public static TObj Color<TObj>(this TObj createObj, string color)
			where TObj: CreatorGraphObj
		{
			createObj.Attributes.Add("color", color);
			return createObj;
		}

		public static TObj FontSize<TObj>(this TObj createObj, int size)
			where TObj : CreatorGraphObj
		{
			createObj.Attributes.Add("fontsize", size.ToString());
			return createObj;
		}

		public static TObj Label<TObj>(this TObj createObj, string label)
			where TObj : CreatorGraphObj
		{
			createObj.Attributes.Add("label", label);
			return createObj;
		}

		public static DotGraphBuilder With<TObj>(this TObj createObj, Action<TObj> addAttribute)
			where TObj : CreatorGraphObj
		{
			addAttribute(createObj);
			return createObj.Builder;
		}
	}

	public abstract class CreatorGraphObj
	{
		internal Dictionary<string, string> Attributes;
		internal DotGraphBuilder Builder;

		public CreatorGraphObj(DotGraphBuilder builder, Dictionary<string, string> nodeAttributes)
		{
			Builder = builder;
			Attributes = nodeAttributes;
		}

		public CreatorEdge AddEdge(string sourceNode, string destinationNode)
		{
			return Builder.AddEdge(sourceNode, destinationNode);
		}

		public CreatorNode AddNode(string nameNode)
		{
			return Builder.AddNode(nameNode);
		}

		public string Build()
		{
			return Builder.Build();
		}
	}

	public class CreatorNode : CreatorGraphObj
	{
		public CreatorNode(DotGraphBuilder builder, Dictionary<string, string> nodeAttributes): base(builder, nodeAttributes)
		{
		}

		internal CreatorNode Shape(NodeShape shape)
		{
			Attributes.Add("shape", shape.ToString().ToLower());
			return this;
		}
	}

	public class CreatorEdge : CreatorGraphObj
	{
		public CreatorEdge(DotGraphBuilder builder, Dictionary<string, string> nodeAttributes) : base(builder, nodeAttributes)
		{
		}

		internal CreatorEdge Weight(int weight)
		{
			Attributes.Add("weight", weight.ToString());
			return this;
		}
	}
}