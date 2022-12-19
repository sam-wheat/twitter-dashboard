using Dashboard.Model;

namespace Dashboard.Domain;

public interface ISerializationWorker
{
    Tweet Deserialize(string json);
}