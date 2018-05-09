<meta name="viewport" content="width=device-width, initial-scale=1">
<link rel="stylesheet" href="github-markdown.css">
<link rel="stylesheet" href="syntax.css">
<style>
	.markdown-body {
		box-sizing: border-box;
		min-width: 200px;
		max-width: 980px;
		margin: 0 auto;
		padding: 45px;
	}

	@media (max-width: 767px) {
		.markdown-body {
			padding: 15px;
		}
	}
</style>

<div class="markdown-body">

# Overview

This library provides an **adjacency list based graph** implementation. Edges are represented by explicit object and can link more than two nodes.

This library is developped as part of an attempt to model and develop user-facing dictionaries with graphs. It is generic enough to be used for other purposes. You can consult the following publications for more details about the data model:

Lecailliez, L, Mangeot, M. (2018). AsiaLex 2018 publication (to be published).

Lecailliez, L. (2016). [Pour une modélisation de dictionnaires de japonais sous forme de graphe.](https://louis.lecailliez.net/dl/memoire_m2_jap_Lecailliez.pdf) [Towards Graph Modeling of Japanese dictionaries] Master thesis, Paris Diderot.

# Main Types

The ```Graph``` class implementing ```IGraph``` contains the vertices and edges of the graph and provides access to annotations.
A graph contains two kind of objects : vertices, implementing the ```IVertex``` interface and edges, implementing ```IEdge```. Both ```IVertex``` and ```IEdge```
derivate from ```IGraphObject```.

Each graph object is associated to an edge or vertex type by linking a ```GraphObjectTypeInfo``` instance which regroup informations about the type.

<table>
    <tr><th>Interface</th><th>Implementation</th></tr>
    <tr><td><code>IGraph</code></td><td><code>Graph</code></td></tr>
    <tr><td><code>IVertex</code></td><td><code>Vertex&lt;T&gt;</code></td></tr>
    <tr><td><code>IEdge</code></td><td><code>HyperEdge</code></td></tr>
<table>

</div>

# Tutorial \#1 Creating a Simple Graph with One Node

In this tutorial, we will create a simple graph containing one node, labeled "Node 1".
Creating the simpliest non-empty graph containing on node involves three steps: (1) creating a data type for the node, (2) instanciating a node, (3) creating the graph and (4) adding
the vertex to the graph.

<img src="schema_1.png"/>

## Vertex Type Creation

First, we create a new vertex type. The following example use the longest constructor available and fills all the object description. The first argument is a ```Guid``` but the constructor here
is a overload allowing a string to be passed instead.

<div class="CodeContainer">
    <ul>
        <li data-lineNumber="1"><span class="InferredIdentifier">GraphObjectTypeInfo</span> demoVertexType = </li>
        <li data-lineNumber="2" class="Alternate">    <span class="Keyword">new</span> <span class="InferredIdentifier">GraphObjectTypeInfo</span>(
        id: <span class="StringLiteral">&quot;0ceb13cc-b23f-4918-9049-09a81807f0cd&quot;</span>,</li>
        <li data-lineNumber="3">        name: <span class="StringLiteral">&quot;Vertex Example Type&quot;</span>, </li>
        <li data-lineNumber="4" class="Alternate">        type: <span class="InferredIdentifier">GraphObjectType</span>.Vertex, </li>
        <li data-lineNumber="5">        direct_content: <span class="Keyword">true</span>, </li>
        <li data-lineNumber="6" class="Alternate">        oriented_edge: <span class="Keyword">false</span>);</li>
        <li data-lineNumber="7">&nbsp;</li>
        <li data-lineNumber="8" class="Alternate">demoVertexType.Description = <span class="StringLiteral">&quot;This type is the first type of the tutorial.&quot;</span>;</li>
    </ul>
</div>

## Node Instanciation

Now that we have a type, we can create a vertex instance. It basically take a string as content, wrapped in a ```SerializableString```. This wrapper can be switched for another when dealing with
more complex content encoding.

<div class="CodeContainer">
    <ul>
        <li data-lineNumber="1"><span class="InferredIdentifier">IVertex</span> demoVertex = <span class="Keyword">new</span> Vertex<<span class="InferredIdentifier">SerializableString</span>>(demoVertexType, <span class="Keyword">new</span> <span class="InferredIdentifier">SerializableString</span>(<span class="StringLiteral">&quot;Node 1&quot;</span>));</li>
    </ul>
</div>

## Graph Creation (step 3 &amp; 4)

The graph instanciation is easy and doesn't require additional explanation.

<div class="CodeContainer">
    <ul>
        <li data-lineNumber="1"><span class="InferredIdentifier">IGraph</span> graph = <span class="Keyword">new</span> <span class="InferredIdentifier">Graph</span>();</li>
        <li data-lineNumber="2">graph.AddVertex(demoVertex);</li>
    </ul>
</div>
&nbsp;

Voilà! The graph is now live and populated with one vertex. In the next tutorial, we will see how we can make use of the graph.

[See the code file](DemoCodeFiles/Tutorial1.cs).

# Tutorial \#2 Creating Non-Oriented Edges

I this tutorial, we will extend the previous graph with two more nodes named "Node 2" and "Node 3". Two edges will be added between the first node and the two new ones.

<img src="schema_2.png"/>

Here two steps are required: (1) creating the edge type and (2) creating the edge itself.

<div class="CodeContainer">
    <ul>
        <li data-lineNumber="5"><span class="InferredIdentifier">IVertex</span> demoVertex2 = <span class="Keyword">new</span> Vertex<<span class="InferredIdentifier">SerializableString</span>>(demoVertexType, <span class="Keyword">new</span> <span class="InferredIdentifier">SerializableString</span>(<span class="StringLiteral">&quot;Node 2&quot;</span>));</li>
                <li data-lineNumber="6"><span class="InferredIdentifier">IVertex</span> demoVertex3 = <span class="Keyword">new</span> Vertex<<span class="InferredIdentifier">SerializableString</span>>(demoVertexType, <span class="Keyword">new</span> <span class="InferredIdentifier">SerializableString</span>(<span class="StringLiteral">&quot;Node 3&quot;</span>));</li>
                <li data-lineNumber="7">graph.AddVertex(demoVertex2);</li>
                <li data-lineNumber="8">graph.AddVertex(demoVertex3);</li>
    </ul>
</div>
&nbsp;

&#9888;&nbsp; <span style="color:orange;">Don't forget to add the nodes to the graph!</span> The rest of this tutorial would work without theses operations, but the number of vertices stored by the graph would be incorrect. The file generated by serializing such a graph would be missing the nodes as well.

## Edge Type Creation

Creating an edge type use the same class ```GraphObjectTypeInfo```seen before. We will however use a shorter constructor. By default, an edge is created as non-oriented.
Be aware of using a different guid value! If you don't, both type will be seen as identical be the system.


<div class="CodeContainer">
    <ul>
        <li data-lineNumber="11"><span class="InferredIdentifier">GraphObjectTypeInfo</span> demoEdgeType =</li>
                <li data-lineNumber="12">    <span class="Keyword">new</span> <span class="InferredIdentifier">GraphObjectTypeInfo</span>(<span class="StringLiteral">&quot;45fede5f-a6bc-4560-a44d-906d5043abdf&quot;</span>,</li>
                <li data-lineNumber="13">        <span class="StringLiteral">&quot;Demo Edge Type&quot;</span>,</li>
                <li data-lineNumber="14">        <span class="InferredIdentifier">GraphObjectType</span>.Edge);</li>
    </ul>
</div>

## Edge Instanciation

Edges, in contrary to nodes, are created by the Graph object. This restricted exists so the graph can keep a reference to every edges it contains.

<div class="CodeContainer">
    <ul>
            <li data-lineNumber="17">graph.CreateEdge(demoEdgeType, demoVertex, demoVertex2);</li>
        <li data-lineNumber="18">graph.CreateEdge(demoEdgeType, demoVertex, demoVertex3);</li>
    </ul>
</div>
&nbsp;

Bravo! The graph now contains three nodes and two edges. In the next tutorial, we will see how we can make use of the graph.

[See the code file](DemoCodeFiles/Tutorial2.cs).

# Tutorial \#3 Querying &amp; Traversing the Graph

Now that we have a graph, we may want to perform search inside it.

## Indexed Strings

The graph maintains various indexes. By searching a node in the graph, it will rebuild its indexes if they are dirty; that is if a node was added since the last build. By default, it will
create an index named "" (the empty string) populated with the indexed strings collected from its vertices. By default, a node returns one indexed string corresponding to its content.

The code line below show that our "Node 1" node will return "Node 1" as its indexed string.

<div class="CodeContainer">
    <ul>
        <li data-lineNumber="1"><span class="InferredIdentifier">Console</span>.WriteLine(<span class="StringLiteral">&quot;Default indexed string of Node 1:&quot;</span> + demoVertex);</li>
    </ul>
</div>

## Performing a Search

We can search one of our node by its content using the ```SearchNode```method. The first argument of the method is the index on which the search is performed. It is an empty string here, which is the default index name.

<div class="CodeContainer">
    <ul>
        <li data-lineNumber="1"><span class="InferredIdentifier">List</span><<span class="InferredIdentifier">IVertex</span>> results = graph.SearchNode(<span class="StringLiteral">""</span>, <span class="StringLiteral">&quot;Node 1&quot;</span>);</li>
        <li>&nbsp;</li>
        <li data-lineNumber="4"><span class="InferredIdentifier">Console</span>.WriteLine(<span class="StringLiteral">&quot;Number of results: &quot;</span> + results.Count);</li>
        <li data-lineNumber="5"><span class="InferredIdentifier">Console</span>.WriteLine(<span class="StringLiteral">&quot;Result node &quot;</span> + results[<span class="NumericLiteral">0</span>]);</li>
    </ul>
</div>
&nbsp;

The list result contains one element: a reference to the first node of our graph. Now let's see how we can access the nodes it links.

## Traversing the Graph

Each node and edge object exposes a ```Links``` property which is a list of ```IEdge``` instances. They materialise the relationship that exists between nodes.
Our first node have, as expected, two edges in his list of links.

<div class="CodeContainer">
    <ul>
        <li data-lineNumber="5"><span class="InferredIdentifier">Console</span>.WriteLine(<span class="StringLiteral">"Number of links from Node 1: "</span> + demoVertex.Links.Count);</ul>
</div>
&nbsp;

Access to the linked nodes is made in two steps: first retrieving the link object, then using it the find the linked node.

<div class="CodeContainer">
    <ul>
        <li data-lineNumber="1">List<<span class="InferredIdentifier">IVertex</span>> linkedNodes = <span class="Keyword">new</span> List<<span class="InferredIdentifier">IVertex</span>>();</li>
        <li data-lineNumber="2"><span class="Keyword">foreach</span> (<span class="InferredIdentifier">IEdge</span> edge <span class="Keyword">in</span> demoVertex.Links)</li>
        <li data-lineNumber="3">{</li>
        <li data-lineNumber="4">	<span class="Keyword">foreach</span> (<span class="InferredIdentifier">IVertex</span> v <span class="Keyword">in</span> edge.GetLinkedObjects(GraphObjectType.Vertex))</li>
        <li data-lineNumber="5">	{</li>
        <li data-lineNumber="6">		<span class="Keyword">if</span> (v.ObjectId != demoVertex.ObjectId)</li>
        <li data-lineNumber="7">		{</li>
        <li data-lineNumber="8">			linkedNodes.Add(v);</li>
        <li data-lineNumber="9">		}</li>
        <li data-lineNumber="10">	}</li>
        <li data-lineNumber="11">}</li>
        <li data-lineNumber="12">linkedNodes.ForEach(n => <span class="InferredIdentifier">Console</span>.WriteLine(n));</li>
    </ul>
</div>
&nbsp;

The code below needs a few comments. First, an edge can actually link both vertices or other edges, that's why ```GetLinkedObjects``` is called with an overload that will return only node instances. Secondly, the list of returned object will contains every nodes that the edge links, including the source node; even when the edge is oriented. This is why we need a filtering condition. This is a feature of the library, is allow to traverse the graph in both directions.

## Get Successors (the Easy Way)

The code previously seen can be used to deal with every kind of nodes and edges. Most of the time, this isn't necessary. If you only need to get successors (or predecessors) of an oriented binary edge, you can use an helper method designed to retrieve them easily.

<div class="CodeContainer">
    <ul>
        <li data-lineNumber="1"><span class="InferredIdentifier">OrientedBinaryEdgeTypeInfo</span> relationship = </li>
        <li data-lineNumber="2">    <span class="Keyword">new</span> <span class="InferredIdentifier">OrientedBinaryEdgeTypeInfo</span>(<span class="StringLiteral">&quot;45fede5f-a6bc-4560-a44d-906d5043abdf&quot;</span>, <span class="StringLiteral">&quot;&quot;</span>, <span class="StringLiteral">&quot;&quot;</span>);</li>
        <li data-lineNumber="3">linkedNodes = demoVertex.Successors(relationship);</li>
    </ul>
</div>
&nbsp;

You need first to create a ```OrientedBinaryEdgeTypeInfo``` with the same identifier than the relationship you want to follow. Then, pass it as the argument of ```Successors```.

[See the code file](DemoCodeFiles/Tutorial3.cs).

# Tutorial #4 Writing and Reading a File

A graph can be saved as an XML file. The file can be manually edited if needed, and loaded back. Note that big graphs can take a signifiant time to load, even on a
desktop computer. You'll need ```using Leger.IO;``` at the top of your file to use the short class names.

## Generating a Graph File

Creating a graph file is straightforward: a ```GraphXmlSerializer``` instance is used to get an XML document representation (```Sytem.Xml.Linq.XDocument``` class)
which is then saved to a given path.

<div class="CodeContainer">
    <ul>
        <li data-lineNumber="1"><span class="InferredIdentifier">GraphXmlSerializer</span> serializer = <span class="Keyword">new</span> <span class="InferredIdentifier">GraphXmlSerializer</span>();</li>
        <li data-lineNumber="2"><span class="InferredIdentifier">XDocument</span> xdoc = <span class="InferredIdentifier">serializer</span>.Serialize(graph);</li>
        <li data-lineNumber="3">xdoc.Save(<span class="StringLiteral">&quot;graph.xml&quot;</span>);</li>
    </ul>
</div>

## Loading a Graph File

       
<div class="CodeContainer">
    <ul>
        <li data-lineNumber="1">graph = <span class="InferredIdentifier">GraphXmlDeserializer</span>.GetGraphInstance(<span class="StringLiteral">"graph.xml"</span>, <span class="Keyword">new</span> <span class="InferredIdentifier">FileLoader</span>());</li>
    </ul>
</div>
&nbsp;

[See the code file](DemoCodeFiles/Tutorial4.cs).