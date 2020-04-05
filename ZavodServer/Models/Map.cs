using Models;

namespace ZavodServer.Models
{
    public class Map
    {
        public string Name { get; set; }
        public Vector3 CenterPosition { get; set; }
        public Vector3 Scale { get; set; }
    }

    public static class MapContainer
    {
        public static Map[] maps = {
            new Map{Name = "SomeMap", CenterPosition = new Vector3{X=200,Y=0,Z=200}, Scale = new Vector3{X=400,Y=0,Z=400}}, 
        };
}
}