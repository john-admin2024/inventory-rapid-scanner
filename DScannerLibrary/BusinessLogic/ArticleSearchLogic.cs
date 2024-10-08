using System.Text;
using DScannerLibrary.DataAccess;
using DScannerLibrary.Models;
using DScannerLibrary.Helpers;
using DbfDataReader;

namespace DScannerLibrary.BusinessLogic;

public class ArticleSearchLogic
{
    private readonly IDataAccess _dataAccess;

    public ArticleSearchLogic(IDataAccess dataAccess)
    {
	 _dataAccess = dataAccess;
    }

    public ArticleModel? GetArticleByBarcode(string articleBarcode)
    {
	 var articles = _dataAccess.ReadData<ArticleModel>($"Select cod, denumire, tva, pret_vanz, cod_bare from articole where cod_bare={articleBarcode}");
	 var article = articles.SingleOrDefault();

	 return article;
    }

    public ArticleModel? GetArticleByBarcode(string articleBarcode, string dbDirectory)
    {
	var _dbDirectory = dbDirectory;

        var options = new DbfDataReaderOptions
        {
            SkipDeletedRecords = true,
            Encoding = Encoding.UTF8
        };

        var dbfName = "ARTICOLE.DBF";
        var dbfPath = $"{DatabaseDirectoryHelper.GetDatabaseDirectory(_dbDirectory)}/{dbfName}";

	try
	{
     	    var dbfDataReader = new DbfDataReader.DbfDataReader(dbfPath, options);
	}
	catch
	{
	    dbfName = "articole.dbf";
	    dbfPath = $"{DatabaseDirectoryHelper.GetDatabaseDirectory(_dbDirectory)}/{dbfName}";
	}

        using (var dbfDataReader = new DbfDataReader.DbfDataReader(dbfPath, options))
        {
            while (dbfDataReader.Read())
            {
                try
                {
                    var article = new ArticleModel()
                    {
                        cod = dbfDataReader.GetString(0),
                        denumire = dbfDataReader.GetString(1),
                        pret_vanz = dbfDataReader.GetDecimal(7),
                        cod_bare = dbfDataReader.GetInt64(14),
                    };

                    if (article.cod_bare == Convert.ToInt64(articleBarcode))
                    {
                        return article;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(
                            $"Verifica toate codurile de bare. E posibil sa fie produse care nu au cod de bare.\n{ex.Message}");
                }
            }
            throw new Exception($"Codul de bare {articleBarcode} nu exista sau e gresit! Verifica la articole!");
        }
    }
}
