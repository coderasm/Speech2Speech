using SpeechToSpeech.Models;
using SpeechToSpeech.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows;

namespace SpeechToSpeech.Services
{
  public class DatabaseService : IDatabaseService
  {
    private Settings settings;
    private ISettingsService settingsService;
    private IWebServiceRepository webServiceRepository;
    private string connectionString = "";
    public string ConnectionString
    {
      get
      {
        return connectionString;
      }
    }

    public DatabaseService(ISettingsService settingsService, IWebServiceRepository webServiceRepository)
    {
      this.settingsService = settingsService;
      settings = settingsService.settings;
      this.webServiceRepository = webServiceRepository;
      Initialize();
    }

    private void Initialize()
    {
      if (File.Exists(settings.generalSettings.Database))
        return;
      connectionString = String.Format("DataSource=\"{0}\";Max Database Size={1};", settings.generalSettings.Database, settings.databaseSettings.MaxDatabaseSize);
      CreateDatabase();
      CreateTables();
      PopulateTables();
    }

    private void CreateDatabase()
    {
      try
      {
        SqlCeEngine en = new SqlCeEngine(connectionString);
        en.CreateDatabase();
        en.Dispose();
      }
      catch (Exception e)
      {
        Console.WriteLine($"Could not create database. Error: {e}");
        MessageBox.Show($"Could not create database. Error: {e}");
      }
    }

    private void CreateTables()
    {
      using (var conn = new SqlCeConnection(connectionString))
      {
        conn.Open();
        using (SqlCeCommand comm = new SqlCeCommand())
        {
          comm.Connection = conn;
          comm.CommandType = CommandType.Text;
          comm.CommandText = "CREATE TABLE voice ([Id] [int] identity(1,1), [ServiceId] [int] NOT NULL, [VoiceId] [nvarchar](100), [Name] [nvarchar](100) not null, [Gender] [nvarchar](50) not null, [Language] [nvarchar](50) not null, [SsmlGender] [tinyint], primary key (Name, ServiceId, Gender, Language))";
          comm.ExecuteNonQuery();
          comm.CommandText = "CREATE TABLE service ([Id] [int] identity(1,1) primary key, [Name] [nvarchar](100) not null)";
          comm.ExecuteNonQuery();
          comm.CommandText = "CREATE TABLE texttospeech ([Id] [int] identity(1,1) primary key, [Text] [ntext] not null, [AudioFile] [nvarchar](200) not null)";
          comm.ExecuteNonQuery();
        }
        conn.Close();
      }
    }

    private void PopulateTables()
    {
      var webServices = new List<WebService>
      {
        new WebService{Name = "Amazon"},
        new WebService{Name = "Google"},
        new WebService{Name = "IBM"}
      };
      webServiceRepository.InsertMultiple(webServices);
    }

    private void CreateTableIndex()
    {
      using (var conn = new SqlCeConnection(connectionString))
      {
        using (SqlCeCommand comm = new SqlCeCommand())
        {
          comm.Connection = conn;
          comm.CommandType = CommandType.Text;
          comm.CommandText = "CREATE INDEX IXgnis ON gnis ([Name]);";
          comm.ExecuteNonQuery();
        }
      }
    }
  }
}
