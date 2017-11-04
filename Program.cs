using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace TwitterNLP
{
    class Program
    {
        static private Properties props;
        static void Main(string[] args)
        {
            System.Console.WriteLine("Started");
            props = new Properties(@"data\props_teste.json");
            TweetExtractor te = new TweetExtractor(props);

            List<string> profiles = new List<string>{"OperacoesRio", "LinhaAmarelaRJ"};
            te.SearchFeatures(profiles);

            Console.WriteLine("Done");
            
            return;
        }

        static void AutoModeCaller(){
            TweetExtractor te = new TweetExtractor(props);
            DBConnection dc = new DBConnection(props.dbCommunityString, props.timeLimit);

            Task tweetStream = Task.Factory.StartNew(() => te.FilteredStreamFeatures());
            Task insertToDB = Task.Factory.StartNew(() => dc.InsertToMySQLFromFile());
            Task.WaitAll(tweetStream, insertToDB);
        }

        static void ManualModeCaller(){
            TweetExtractor te = new TweetExtractor(props);
            Console.WriteLine("Bem-vindo ao modo manual.\nEscolha a opção desejada.");
            Console.WriteLine(
                "1) Configurar arquivo de propriedades.\n " +
                "2) Realizar busca na API do Twitter.\n" +
                "3) Importar tweets do banco de dados.\n"
            );
            int option = Int32.Parse(Console.ReadLine());
            if(option == 1){
                Properties.buildPropertiesFile();
                Console.WriteLine("Arquivo salvo com sucesso.");
            }
            else if(option == 2){
                Console.WriteLine("Insira os perfis que deseja buscar separados por virgula");
                string profilesLine = Console.ReadLine();
                string[] profiles = profilesLine.Split(',', StringSplitOptions.RemoveEmptyEntries);
                List<string> cleanProfiles = new List<string>();
                foreach(string profile in profiles){
                    string cleanProfile = profile.Trim();
                    cleanProfiles.Add(cleanProfile);
                }
                te.SearchFeatures(cleanProfiles);
                Console.WriteLine("Busca terminada.");
            }
            else if(option == 3){

            }
            else if(option == 4){
                return;
            }
            else{
                Console.WriteLine("Opção invalida. Tente novamente.");
            }
        }
    }
}
