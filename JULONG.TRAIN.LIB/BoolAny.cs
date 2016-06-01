using System.Diagnostics;
namespace JULONG.TRAIN.LIB
{
    /// <summary>
    /// 泛型消息处理
    /// </summary>
    public struct BoolAny<T>
    {
        /// <summary>
        /// 内容
        /// </summary>
        public T t { get; private set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool state { get; private set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string message { get; private set; }

        /// <summary>
        /// 堆栈跟踪
        /// </summary>
        public StackTrace stackTrace { get; private set; }

        public BoolAny(bool state, T t, string message)
            : this()
        {
            this.state = state;
            this.t = t;
            this.message = message;
            this.stackTrace = new StackTrace(true);
        }

        /// <summary>
        /// 创建一个成功的消息
        /// </summary>
        /// <param nickname="message"></param>
        /// <returns></returns>
        public static BoolAny<T> succeed(T t = default(T), string message = null)
        {
            return new BoolAny<T>(true, t, message);
        }
        /// <summary>
        /// 创建一个失败的消息
        /// </summary>
        /// <param nickname="message"></param>
        /// <returns></returns>
        public static BoolAny<T> fail(T t = default(T), string message = null)
        {
            return new BoolAny<T>(false, t, message);
        }

        /// <summary>
        /// 创建一个失败的消息
        /// </summary>
        /// <param nickname="message"></param>
        /// <returns></returns>
        public static BoolAny<T> fail(string message = null)
        {
            return new BoolAny<T>(false, default(T), message);
        }

        /// <summary>
        /// bool的显式转换
        /// </summary>
        /// <param nickname="ba"></param>
        /// <returns></returns>
        public static implicit operator bool(BoolAny<T> ba)
        {
            return ba.state;
        }

        /// <summary>
        /// 内容的显式转换
        /// </summary>
        /// <param nickname="bm"></param>
        /// <returns></returns>
        public static implicit operator T(BoolAny<T> ba)
        {
            return ba.t;
        }
    }
}