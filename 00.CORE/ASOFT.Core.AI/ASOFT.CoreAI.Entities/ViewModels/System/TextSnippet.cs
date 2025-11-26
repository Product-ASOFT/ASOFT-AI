// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;

namespace ASOFT.CoreAI.Entities;

/// <summary>s
/// Data model for storing a section of text with an embedding and an optional reference link.
/// </summary>
/// <typeparam name="TKey">The type of the data model key.</typeparam>
public sealed class TextSnippet
{
    [VectorStoreKey]
    public Guid Key { get; set; }

    [TextSearchResultValue]
    [VectorStoreData]
    public string? Text { get; set; }

    [TextSearchResultName]
    [VectorStoreData]
    public string? ReferenceDescription { get; set; }

    [TextSearchResultLink]
    [VectorStoreData]
    public string? ReferenceLink { get; set; }

    [VectorStoreVector(1536)]
    public string? TextEmbedding { get; set; }

    [VectorStoreVector(1536)]
    public float[] EmbeddingVector { get; set; }

    // Các trường bổ sung để quản lý metadata và phân loại

    [VectorStoreData]
    public string? DatasetId { get; set; }  // ID tập dữ liệu

    [VectorStoreData]
    public string? Module { get; set; }     // Tên module nghiệp vụ

    [VectorStoreData]
    public string? FileType { get; set; }   // Loại file: pdf, doc, xls, ...

    [VectorStoreData]
    public string? FileName { get; set; }   // Tên file gốc

    [VectorStoreData]
    public DateTime CreatedAt { get; set; } // Thời gian tạo, dùng để lọc/sắp xếp
}