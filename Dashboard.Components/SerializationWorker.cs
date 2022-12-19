namespace Dashboard.Components;

public class SerializationWorker : ISerializationWorker
{
    public SerializationWorker()
    {

    }

    public Tweet Deserialize(string json)
    {
        if (string.IsNullOrEmpty(json))
            throw new ArgumentNullException(nameof(json));

        var node = JsonNode.Parse(json)["data"];

        if (node is null)
            return null;

        return new Tweet
        {
            ID = (string)node["id"],
            Text = (string)node["text"],
            Hashtags = (node["entities"]["hashtags"]?.AsArray().Select(x => (string)x["tag"]))?.ToArray()
        };
    }
}
