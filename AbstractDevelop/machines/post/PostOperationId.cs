﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.post
{
    /// <summary>
    /// Кодирует возможные операции машины Поста.
    /// </summary>
    public enum PostOperationId : byte
    {
        /// <summary>
        /// Сместить каретку влево.
        /// </summary>
        Left,
        /// <summary>
        /// Сместить каретку вправо.
        /// </summary>
        Right,
        /// <summary>
        /// Стереть метку.
        /// </summary>
        Erase,
        /// <summary>
        /// Установить метку.
        /// </summary>
        Place,
        /// <summary>
        /// Условный переход.
        /// </summary>
        Decision,
        /// <summary>
        /// Завершить выполнение программы.
        /// </summary>
        Stop
    }
}
