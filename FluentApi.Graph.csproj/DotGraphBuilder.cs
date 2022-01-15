using System;
using System.Collections.Generic;
using System.Globalization;

namespace FluentApi.Graph
{
    public enum NodeShape
    {
        Box,
        Ellipse
    }
    
    public interface IGraphBuilder
    {
        GraphNodeBuilder AddNode(string nodeName);
        
        GraphEdgeBuilder AddEdge(string sourceNode, string destinationNode);
        
        string Build();
    }
    
    public class DotGraphBuilder : IGraphBuilder
    {
        private readonly Graph graph;

        private DotGraphBuilder(string graphName, bool directed) 
            => graph = new Graph(graphName, directed, false);

        public GraphNodeBuilder AddNode(string nodeName) 
            => new GraphNodeBuilder(graph.AddNode(nodeName), this);
        
        
        public GraphEdgeBuilder AddEdge(string sourceNode, string destinationNode)
            => new GraphEdgeBuilder(graph.AddEdge(sourceNode, destinationNode), this);

        public string Build() => graph.ToDotFormat();
        
        public static IGraphBuilder DirectedGraph(string graphName) 
            => new DotGraphBuilder(graphName,true);

        public static IGraphBuilder UndirectedGraph(string graphName) 
            => new DotGraphBuilder(graphName, false);
    }
    
    public class GraphBuilder : IGraphBuilder
    {
        protected readonly IGraphBuilder parent;

        public GraphBuilder(IGraphBuilder parent) => this.parent = parent;

        public GraphNodeBuilder AddNode(string nodeName) => parent.AddNode(nodeName);

        public GraphEdgeBuilder AddEdge(string sourceNode, string destinationNode) 
            => parent.AddEdge(sourceNode, destinationNode);

        public string Build() => parent.Build();
    }
    
    public class GraphEdgeBuilder : GraphBuilder
    {
        private readonly GraphEdge edge;

        public GraphEdgeBuilder(GraphEdge edge, IGraphBuilder parent) : base(parent) 
            => this.edge = edge;

        public IGraphBuilder With(Action<EdgeCommonAttributesConfig> applyAttributes)
        {
            applyAttributes(new EdgeCommonAttributesConfig(edge));
            return parent;
        }
    }
    
    public class GraphNodeBuilder : GraphBuilder
    {
        private readonly GraphNode node;

        public GraphNodeBuilder(GraphNode node, IGraphBuilder parent) : base(parent)
            => this.node = node;

        public IGraphBuilder With(Action<NodeCommonAttributesConfig> applyAttributes)
        {
            applyAttributes(new NodeCommonAttributesConfig(node));
            return parent;
        }
    }
    
    public class CommonAttributesConfig<TConfig>
        where TConfig : CommonAttributesConfig<TConfig>
    {
        private readonly IDictionary<string, string> attributes;

        public CommonAttributesConfig(IDictionary<string, string> attributes) 
            => this.attributes = attributes;

        public TConfig Label(string label)
        {
            attributes["label"] = label;
            return (TConfig) this;
        }
        
        public TConfig FontSize(float sizeInPt)
        {
            attributes["fontsize"] = sizeInPt.ToString(CultureInfo.InvariantCulture);
            return (TConfig) this;
        }
        
        public TConfig Color(string color)
        {
            attributes["color"] = color;
            return (TConfig) this;
        }
    }
    
    public class NodeCommonAttributesConfig : CommonAttributesConfig<NodeCommonAttributesConfig>
    {
        private readonly GraphNode node;

        public NodeCommonAttributesConfig(GraphNode node) : base(node.Attributes) 
            => this.node = node;

        public NodeCommonAttributesConfig Shape(NodeShape shape)
        {
            node.Attributes["shape"] = shape.ToString().ToLowerInvariant();
            return this;
        }
    }
    
    public class EdgeCommonAttributesConfig : CommonAttributesConfig<EdgeCommonAttributesConfig>
    {
        private readonly GraphEdge edge;

        public EdgeCommonAttributesConfig(GraphEdge edge) : base(edge.Attributes) => this.edge = edge;
        
        public EdgeCommonAttributesConfig Weight(double weight)
        {
            edge.Attributes["weight"] = weight.ToString(CultureInfo.InvariantCulture);
            return this;
        }
    }
}