using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryModuleManager2.ClassificationEntity
{
    /// <summary>
    /// Тип ссылки
    /// </summary>
    public enum LinkType
    {

        /// <summary>
        /// Неизвестно или ошибка (0)
        /// </summary>
        Unknown,
        /// <summary>
        /// ссылка на хранилище
        /// </summary>
        StorageLink,
        /// <summary>
        /// ссылка на файл документа
        /// </summary>
        DocumentFileLink,
        /// <summary>
        /// ссылка на файл изображения
        /// </summary>
        PictureFileLink,
        /// <summary>
        /// ссылка на запись сущности
        /// </summary>
        EntityRecordLink,
        /// <summary>
        /// ссылка на запись документа
        /// </summary>
        DocumentRecordLink,
        /// <summary>
        /// ссылка на запись изображения
        /// </summary>
        PictureRecordLink,
    }
}
