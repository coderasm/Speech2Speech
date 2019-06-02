using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Windows;

namespace SpeechToSpeech.Services
{
  public class DatabaseService
  {
    private static Settings settings;
    private static string connectionString;

    public static void Initialize(Settings settings)
    {
      DatabaseService.settings = settings;
      connectionString = String.Format("DataSource=\"{0}\";Max Database Size=3000;", settings.generalSettings.Database);
      if (File.Exists(settings.generalSettings.Database))
        return;
      CreateDatabase();
      CreateTables();
    }

    private static void CreateDatabase()
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

    private static void CreateTables()
    {
      using (var conn = new SqlCeConnection(connectionString))
      {
        conn.Open();
        using (SqlCeCommand comm = new SqlCeCommand())
        {
          comm.Connection = conn;
          comm.CommandType = CommandType.Text;
          comm.CommandText = "CREATE TABLE voice ([Service] [nvarchar](100) NOT NULL, [Id] [nvarchar](100), [Name] [nvarchar](100), [Gender] [nvarchar](50), [Language] [nvarchar](50), [SsmlGender] [tinyint])";
          comm.ExecuteNonQuery();
        }
        conn.Close();
      }
    }

    private static void CreateTableIndex()
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
