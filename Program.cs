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
            if(args.Length > 0 ){
                if(args[0].Equals("-a")){
                    if(args.Length > 2){
                        if(args[1].Equals("-p") && !args[2].Equals("")){
                            props = new Properties(args[2]);
                            Console.WriteLine("Starting auto-mode...\n");
                            AutoModeCaller();
                        }
                        else{
                            Console.WriteLine("Indique o arquivo de propriedades usando -p <FilePath>");
                        }   
                    }
                    else{
                        Console.WriteLine("Indique o arquivo de propriedades usando -p <FilePath>");
                    }
                }
                else if(args[0].Equals("-m")){
                    if(args.Length > 2){
                        if(args[1].Equals("-p") && !args[2].Equals("")){
                            props = new Properties(args[2]);
                            Console.WriteLine("Starting manual-mode...\n");
                            ManualModeCaller();
                        }
                        else{
                        Console.WriteLine("Indique o arquivo de propriedades usando -p <FilePath>");
                        }
                    }
                    else{
                        Console.WriteLine("É necessário um arquivo de propriedades, deseja criar um agora? [S/N]");
                        string option = Console.ReadLine();
                        if(option.Equals("s", StringComparison.InvariantCultureIgnoreCase)){
                            Properties.buildPropertiesFile();
                        }
                        else if(option.Equals("n", StringComparison.InvariantCultureIgnoreCase)){
                            return;
                        }
                        else{
                            Console.WriteLine("Opção invalida, tente novamete.");
                        }
                    }  
                }
            }
            else{
                Console.WriteLine("Erro: Insira os parâmetros segundo a documentação.");
            }


            // System.Console.WriteLine("Started");
            // props = new Properties(@"data\props_teste.json");
            // TweetExtractor te = new TweetExtractor(props);

            // List<string> profiles = new List<string>{"OperacoesRio", "LinhaAmarelaRJ"};
            // te.SearchFeatures(profiles);

            // Console.WriteLine("Done");
            
            // return;
        }

        static void AutoModeCaller(){

            if(VerifyPropsForAutoMode()){
                TweetExtractor te = new TweetExtractor(props);
                DBConnection dc = new DBConnection(props.dbCommunityString, props.timeLimit);

                Task tweetStream = Task.Factory.StartNew(() => te.FilteredStreamFeatures());
                Task insertToDB = Task.Factory.StartNew(() => dc.InsertToMySQLFromFile());
                Task.WaitAll(tweetStream, insertToDB);
            }
            else{
                Console.WriteLine("Há propriedades obrigatórias ausentes no arquivo de propriedades.");
            }
        }

        static bool VerifyPropsForAutoMode(){
            if(props.userAccessToken != null &&
                 props.userAcessSecret != null &&
                 props.consumerKey != null &&
                 props.consumerSecret != null &&
                 props.boundingBoxBottomLeft != null &&
                 props.boundingBoxTopRight != null &&
                 props.dbCommunityString != null){
                     return true;
                 }
                else{
                    return false;
                }
        }

        static void ManualModeCaller(){
            TweetExtractor te = new TweetExtractor(props);
            DBConnection dc = new DBConnection(props.dbCommunityString, props.timeLimit);
            Console.WriteLine("Bem-vindo ao modo manual.\nEscolha a opção desejada.\n");
            while(true){
                Console.WriteLine(
                    "1) Configurar arquivo de propriedades.\n" +
                    "2) Realizar busca na API do Twitter.\n" +
                    "3) Importar tweets do banco de dados.\n" +
                    "4) Sair\n"
                );
                int option = Int32.Parse(Console.ReadLine());
                if(option == 1){
                    Properties.buildPropertiesFile();
                    Console.WriteLine("Arquivo salvo com sucesso.");
                }
                else if(option == 2){
                    if(props.userAccessToken != null && props.userAcessSecret != null && props.consumerKey != null && props.consumerSecret != null){
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
                    else{
                        Console.WriteLine("O seu arquivo de propridedes não contém as chaves da API do Twitter. " +
                        "Utilize a opção 1 para criar um novo arquivo com as chaves.");
                    }
                }
                else if(option == 3){
                    Console.WriteLine("Insira um intervalo de IDs de tweets no banco que deseja exportar.");
                    string range = Console.ReadLine();
                    string[] numbers = range.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if(numbers.Length == 2 && Int32.TryParse(numbers[0], out int number1) && Int32.TryParse(numbers[1], out int number2)){
                        dc.ReadGivenRange(number1, number2);
                    }
                    else{
                        Console.WriteLine("Números invalidos. Tente novamente.");
                    }
                }
                else if(option == 4){
                    return;
                }
                else{
                    Console.WriteLine("Opção inválida. Tente novamente.");
                }
            }
        }
    }
}
