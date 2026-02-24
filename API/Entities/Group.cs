using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    //For presence hub we had stored the presence in memory using the concurrent dictionary, but that was not scalable, so to make the message hub scalable just for the learning purpose I will be storing the groups in the database, so now it doesn't matter how many servers we have or we have added the load balacer our groups will be fetched from the database
    public class Group(string name)
    {
        // we gonna skip the creation of an ID because all of our group will have the unique name
        [Key] //By making it as the primary key it means it is automatically indexed
        public string Name { get; set; } = name;

        //Nav Properties
        public ICollection<Connection> Connecitons { get; set; } = [];
    }
}
