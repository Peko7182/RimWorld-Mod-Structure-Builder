using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace RimWorld_Mod_Structure_Builder.Utils;

/// <summary>
/// Utility class for working with XML.
/// </summary>
public static class XmlUtils
{
    /// <summary>
    /// Checks whether two XML documents are equal.
    /// </summary>
    /// <remarks>
    /// The comparison is done by comparing the outer XML of the two documents.
    /// </remarks>
    /// <param name="doc1">The first document.</param>
    /// <param name="doc2">The second document.</param>
    /// <returns><c>true</c> if the documents are equal, <c>false</c> otherwise.</returns>
    public static bool AreXmlDocumentsEqual(XmlDocument doc1, XmlDocument doc2)
    {
        if (doc1 == null && doc2 == null)
            return true;
        if (doc1 == null || doc2 == null)
            return false;
        return doc1.OuterXml.Equals(doc2.OuterXml, StringComparison.Ordinal);
    }
    
    /// <summary>
    /// Retrieves the version of the specified source object.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object.</typeparam>
    /// <param name="source">The source object from which to retrieve the version.</param>
    /// <returns>
    /// A string representation of the version if the "Version" property is found and not null;
    /// otherwise, <c>null</c>.
    /// </returns>
    private static string GetVersion<TSource>(this TSource source) =>
        source?.GetType().GetProperty("Version")?.GetValue(source)?.ToString();

    /// <summary>
    /// Adds a simple node to the specified parent.
    /// </summary>
    /// <param name="parent">The parent node to add the new node to.</param>
    /// <param name="name">The name of the new node.</param>
    /// <param name="value">The value of the new node. If this is null or empty, no node is added.</param>
    public static void AddSimpleNode(XmlElement parent, string name, string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        
        var node = parent.OwnerDocument?.CreateElement(name);
        if (node == null) return;

        node.InnerText = value;
        parent.AppendChild(node);
    }

    /// <summary>
    /// Adds a list of values as child nodes to the specified parent element. 
    /// If the <paramref name="inputValues"/> collection contains only one value, a single node is created with the name specified by <paramref name="singleName"/>. 
    /// If there are multiple values, they are added as child nodes under a list node with the name specified by <paramref name="listName"/>.
    /// Sorry, if very confusing
    /// </summary>
    /// <typeparam name="T">The type of the elements in the input values.</typeparam>
    /// <param name="parent">The parent XML element to which nodes will be added.</param>
    /// <param name="singleName">The name of the node to be added if there is only one value.</param>
    /// <param name="listName">The name of the list node if there are multiple values.</param>
    /// <param name="inputValues">The list of input values to add as nodes.</param>
    /// <remarks>
    /// If there is only one value in <paramref name="inputValues"/>, it is added as a single node with the name <paramref name="singleName"/>.
    /// If there is more than one value in <paramref name="inputValues"/>, they are added as child nodes of "li" under a list node with the name <paramref name="listName"/>.
    /// </remarks>
    public static void AddListNode<T>(XmlElement parent, string singleName, string listName, IEnumerable<T> inputValues)
    {
        if (inputValues == null) return;
        var values = inputValues.ToList();
        
        if (!values.Any()) return;
        var ownerDocument = parent.OwnerDocument;
        if (ownerDocument == null) return;

        if (values.Count() == 1)
        {
            AddSimpleNode(parent, singleName, values.First().ToString());
        }
        else
        {
            AddListNode(parent, listName, values);
        }
    }

    /// <summary>
    /// Adds a list of values as child nodes to the specified parent element.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the input values.</typeparam>
    /// <param name="parent">The parent XML element to which the list node will be added.</param>
    /// <param name="listName">The name of the list node to be created.</param>
    /// <param name="inputValues">The list of input values to add as child nodes.</param>
    /// <remarks>
    /// Each value in <paramref name="inputValues"/> is added as a child node with the name "li" under the created list node.
    /// </remarks>
    public static void AddListNode<T>(XmlElement parent, string listName, IEnumerable<T> inputValues)
    {
        if (inputValues == null) return;
        var values = inputValues.ToList();
        
        if (!values.Any()) return;
        var ownerDocument = parent.OwnerDocument;
        if (ownerDocument == null) return;

        var listNode = ownerDocument.CreateElement(listName);
        parent.AppendChild(listNode);

        foreach (var value in values)
        {
            var node = ownerDocument.CreateElement("li");
            node.InnerText = value.ToString();
            listNode.AppendChild(node);
        }
    }

    /// <summary>
    /// Adds a list of values as child nodes to the specified parent element, using version information.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the input values.</typeparam>
    /// <param name="parent">The parent XML element to which the nodes will be added.</param>
    /// <param name="singleName">The name of the node to be created if there is only one input value.</param>
    /// <param name="listName">The name of the list node to be created if there are multiple input values.</param>
    /// <param name="versionSelector">A function to select the version string from each input value.</param>
    /// <param name="valueSelect">A function to select the string representation of each input value.</param>
    /// <param name="inputValues">The list of input values to process.</param>
    /// <remarks>
    /// If there is only one input value, a single node is added with the name <paramref name="singleName"/>. 
    /// For multiple input values, a list node with the name <paramref name="listName"/> is created, and each 
    /// input value is added as a child node using its version as the node name.
    /// </remarks>
    public static void AddListVersionNode<T>(XmlElement parent, string singleName, string listName,
        Func<T, string> versionSelector, Func<T, string> valueSelect, IEnumerable<T> inputValues)
    {
        if (inputValues == null) return;
        var values = inputValues.ToList();
        
        if (!values.Any()) return;
        var ownerDocument = parent.OwnerDocument;
        if (ownerDocument == null) return;

        if (values.Count() == 1)
        {
            AddSimpleNode(parent, singleName, valueSelect(values.First()));
        }
        else
        {
            var listNode = ownerDocument.CreateElement(listName);

            foreach (var value in values)
            {
                var version = versionSelector(value);
                if (string.IsNullOrEmpty(version)) continue;
                
                var versionNode = ownerDocument.CreateElement(version);
                versionNode.InnerText = valueSelect(value);
                listNode.AppendChild(versionNode);
            }
        }
    }
    
    /// <summary>
    /// Adds nodes to the specified parent XML element based on the input data, 
    /// organizing them by version if applicable.
    /// </summary>
    /// <typeparam name="TSource">The type of the source data elements.</typeparam>
    /// <typeparam name="TResult">The type of the selected data elements.</typeparam>
    /// <param name="parent">The parent XML element to which the nodes will be added.</param>
    /// <param name="listName">The name of the list node to be created.</param>
    /// <param name="inputData">The collection of source data to process.</param>
    /// <param name="selector">A function to select data from the source elements.</param>
    /// <remarks>
    /// Each input element is processed to create an XML node, using the version 
    /// information if available. Nodes are organized under a list node, with a 
    /// version-specific list node created if any input elements contain version 
    /// information.
    /// </remarks>
    public static void AddByVersionNode<TSource, TResult>(XmlElement parent, string listName, IEnumerable<TSource> inputData,
        Func<TSource, TResult> selector)
    {
        if (inputData == null) return;
        var data = inputData.ToList();
        
        if (!data.Any()) return;
        var ownerDocument = parent.OwnerDocument;
        if (ownerDocument == null) return;

        // Get the selected data
        var selectedData = data.Select(selector).ToList();
        if (!selectedData.Any()) return;

        var listNode = ownerDocument.CreateElement(listName);
        var isVersioned = selectedData.Any(d => d.GetVersion() != null);
        XmlElement versionedListNode = null;

        if (isVersioned)
            versionedListNode = ownerDocument.CreateElement($"{listName}ByVersion");

        parent.AppendChild(isVersioned ? versionedListNode : listNode);

        foreach (var dataItem in selectedData)
        {
            var node = ownerDocument.CreateElement(dataItem.GetVersion() ?? "li");
            
            // Single property (<li>value</li>)
            if (dataItem.GetType().GetProperties().Count(p => p.Name != "Version") <= 1)
                node.InnerText = dataItem.GetType().GetProperties().First(p => p.Name != "Version").GetValue(dataItem)?.ToString() ?? string.Empty;
            // Multiple properties (<li>name=value</li>)
            else
                foreach (var propertyInfo in dataItem.GetType().GetProperties().Where(p => p.Name != "Version"))
                    AddSimpleNode(node, propertyInfo.Name, propertyInfo.GetValue(dataItem)?.ToString());

            (dataItem.GetVersion() == null ? listNode : versionedListNode)?.AppendChild(node);
        }
    }
}