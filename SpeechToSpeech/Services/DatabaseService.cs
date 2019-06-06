using System;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows;

namespace SpeechToSpeech.Services
{
  public class DatabaseService: IDatabaseService
  {
    private Settings settings;
    private ISettingsService settingsService;
    private string connectionString;

    public DatabaseService(ISettingsService settingsService)
    {
      this.settingsService = settingsService;
      settings = settingsService.settings;
      Initialize();
    }

    private void Initialize()
    {
      if (File.Exists(settings.generalSettings.Database))
        return;
      connectionString = String.Format("DataSource=\"{0}\";Max Database Size=3000;", settings.generalSettings.Database);
      CreateDatabase();
      CreateTables();
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
          comm.CommandText = "CREATE TABLE voice ([Service_Id] [int] NOT NULL, [Id] [nvarchar](100), [Name] [nvarchar](100) not null, [Gender] [nvarchar](50) not null, [Language] [nvarchar](50) not null, [SsmlGender] [tinyint])";
          comm.ExecuteNonQuery();
          comm.CommandText = "CREATE TABLE service ([Id] [int] identity(1,1) primary key, [Name] [nvarchar](100) not null)";
          comm.ExecuteNonQuery();
          comm.CommandText = "CREATE TABLE texttospeech ([Id] [int] identity(1,1) primary key, [Text] [ntext] not null, [AudioFile] [nvarchar](200) not null)";
          comm.ExecuteNonQuery();
        }
        conn.Close();
      }
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
