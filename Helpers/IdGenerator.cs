using NanoidDotNet;

namespace API_WebH3.Helpers;


public static class IdGenerator
{
    private static int SIZE_COURSE = 8;
    private static int SIZE_LESSON = 12;
    private static int SIZE_ORDER = 12;
    private static int SIZE_ORDER_DETAIL = 12;
    private static int SIZE_QUIZ = 12;
    
    /// <summary>
    /// Sinh ID mặc định 12 ký tự
    /// </summary>
    public static string NewId(int size = 12)
    {
        return Nanoid.Generate(size: size);
    }

    /// <summary>
    /// Sinh ID với bảng ký tự tùy chỉnh
    /// </summary>
    public static string NewCustomId(string alphabet, int size)
    {
        return Nanoid.Generate(alphabet, size);
    }

    /// <summary>
    /// Sinh ID cho Khóa học
    /// </summary>
    public static string IdCourse()
    {
        return Nanoid.Generate(size: SIZE_COURSE);
    }
    
    /// <summary>
    /// Sinh ID cho bài học
    /// </summary>
    public static string IdLesson()
    {
        return Nanoid.Generate(size: SIZE_LESSON);
    }
    
    /// <summary>
    /// Sinh ID Order
    /// </summary>
    public static string IdOrder()
    {
        return Nanoid.Generate(size: SIZE_ORDER);
    }
    
    /// <summary>
    /// Sinh ID Order Detail
    /// </summary>
    public static string IdOrderDetail()
    {
        return Nanoid.Generate(size: SIZE_ORDER_DETAIL);
    }
    
    /// <summary>
    /// Sinh ID Quiz
    /// </summary>
    public static string IdQuiz()
    {
        return Nanoid.Generate(size: SIZE_QUIZ);
    }
}
