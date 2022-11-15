namespace Server.HttpServer
{
    interface IHttpServer
    {
        void StartListening();

        void StopListening();
    }
}
