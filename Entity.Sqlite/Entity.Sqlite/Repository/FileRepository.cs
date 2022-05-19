﻿using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using My.Demo.FileUpload.Model;
using File = My.Demo.FileUpload.Model.File;

namespace My.Demo.FileUpload.Entity.Sqlite
{
    public class FileRepository : IFileRepository
    {
        private readonly ILogger<FileRepository> _logger;

        public FileRepository(ILogger<FileRepository> logger)
        {
            _logger = logger;
        }

        public List<File> GetList(FileFilter filter, ResultPaging paging)
        {
            const string func = "GetList";
            try
            {
                List<File> list = new();
                using var connection = new SqliteConnection(EntityConfigSection.ConnectionString);
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    SELECT FileId, FileGuid, FileName, MimeType, FileSize, Description, CreateDate, CreateUserId
                    FROM File
                ";

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int? fileId = reader.GetFieldValue<int?>(0);
                    string fileGuid = reader.GetFieldValue<string>(1);
                    string fileName = reader.GetFieldValue<string>(2);
                    string mimeType = reader.GetFieldValue<string>(3);
                    int fileSize = reader.GetFieldValue<int>(4);
                    string description = reader.GetFieldValue<string>(5);
                    DateTime createDate = reader.GetFieldValue<DateTime>(6);
                    int? createUserId = reader.GetFieldValue<int?>(7);

                    list.Add(new File()
                    {
                        FileId = fileId,
                        FileGuid = fileGuid,
                        FileName = fileName,
                        MimeType = mimeType,
                        FileSize = fileSize,
                        Description = description,
                        CreateDate = createDate,
                        CreateUserId = createUserId
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{0}: Exception caught.", func);
                throw;
            }
        }

        public File GetData(int fileId)
        {
            throw new NotImplementedException();
        }

        public File SaveData(File file)
        {
            const string func = "SaveData";
            try
            {
                List<File> list = new();
                using var connection = new SqliteConnection(EntityConfigSection.ConnectionString);
                connection.Open();
                if (!file.FileId.HasValue)
                {
                    var command = connection.CreateCommand();
                    command.CommandText =
                    @"
                        INSERT INTO File (FileGuid, FileName, MimeType, FileSize, Description, CreateDate, CreateUserId, IsTemp)
                        VALUES (@fileGuid, @fileName, @mimeType, @fileSize, @description, DATE('now'), @createUserId, @isTemp);
                        SELECT FileId, CreateDate FROM File WHERE FileId = LAST_INSERT_ROWID();
                    ";
                    command.Parameters.AddWithValue("@fileGuid", file.FileGuid);
                    command.Parameters.AddWithValue("@fileName", file.FileName);
                    command.Parameters.AddWithValue("@mimeType", file.MimeType);
                    command.Parameters.AddWithValue("@fileSize", file.FileSize);
                    command.Parameters.AddWithValue("@description", !string.IsNullOrEmpty(file.Description) ? file.Description : DBNull.Value);
                    command.Parameters.AddWithValue("@createUserId", file.CreateUserId.HasValue ? file.CreateUserId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@isTemp", file.IsTemp);
                    command.Prepare();

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int? fileId = reader.GetFieldValue<int?>(0);
                        DateTime createDate = reader.GetFieldValue<DateTime>(1);
                        file.FileId = fileId;
                        file.CreateDate = createDate;
                    }
                }
                else
                {

                }

                return file;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{0}: Exception caught.", func);
                throw;
            }
        }

        public bool DeletData(int fileId)
        {
            throw new NotImplementedException();
        }
    }
}