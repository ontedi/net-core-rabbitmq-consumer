using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQConsumer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            string strKisiListesi = string.Empty;
            var cfBaglantiBilgileri = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            using (IConnection cfBaglanti = cfBaglantiBilgileri.CreateConnection())
            using (IModel chnKanal = cfBaglanti.CreateModel())
            {
                chnKanal.QueueDeclare
                (
                    queue: "bilgilendirme-mesajlari-2022-01-15",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                var ebcKuyruklar = new EventingBasicConsumer(chnKanal);

                ebcKuyruklar.Received += (model, mq) =>
                {
                    var MesajGovdesi = mq.Body;
                    strKisiListesi = Encoding.UTF8.GetString(MesajGovdesi.ToArray());
                };

                chnKanal.BasicConsume
                (
                    queue: "bilgilendirme-mesajlari-2022-01-15",
                    autoAck: false, // true ise mesaj otomatik olarak kuyruktan silinir
                    consumer: ebcKuyruklar
                );
            }
            TempData["KisiListesi"] = strKisiListesi;
            return View();
        }
    }
}
