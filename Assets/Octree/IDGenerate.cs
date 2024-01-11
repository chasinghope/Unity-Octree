public class IDGenerate
{
    public static int id_number = 0;

    public static int GetId()
    {
        return id_number++;
    }


}