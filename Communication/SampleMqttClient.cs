using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using Wirepas_ELA_Mesh_Sample.Model;

/**
 * \namespace Wirepas_ELA_Mesh_Sample.Communication
 * \brief namespace dedicated to the communication
 */
namespace Wirepas_ELA_Mesh_Sample.Communication
{
    /**
     * \class SampleMqttClient
     * \brief simple implementation as sample for MQTT client using nuget package
     */
    public class SampleMqttClient : MqttClient
    {
        /** \brief mqtt broker info */
        public MqttBrokerInfo BrokerInfo { get; }

        /** \brief ID associated to the client */
        public string ID { get; }

        /**
         * \brief constructor 
         * \param [in] brokerInfo : informations relative to the broker
         * \param [in] secure : define if the communication is secure or not
         * \param [in] caCert : 
         * \param [in] clientCert : 
         * \param [in] ssProtocol : 
         */
        public SampleMqttClient(MqttBrokerInfo brokerInfo, bool secure, X509Certificate caCert, X509Certificate clientCert, MqttSslProtocols sslProtocol)
            : base(brokerInfo.broker_address, brokerInfo.broker_port, secure, caCert, clientCert, sslProtocol)
        {
            BrokerInfo = brokerInfo;
            ID = Guid.NewGuid().ToString();
        }

        /**
         * \fn Connect
         * \brief Connection function
         */
        public void Connect()
        {
            if (IsConnected) return;

            if (!string.IsNullOrWhiteSpace(BrokerInfo.Login) && !string.IsNullOrWhiteSpace(BrokerInfo.Password))
                Connect(ID, BrokerInfo.Login, BrokerInfo.Password);
            else
                Connect(ID);
        }

        #region Subscribe - Unsubscribe
        /**
         * \fn Subscribe
         * \brief subscribe to mqtt flow
         * \param [in] subscribeInfo : mqtt subscribe informations
         * \param [in] callBack : callback for event association
         * \return success or failed
         */
        public bool Subscribe(MqttSubscribeInfo subscribeInfo, MqttMsgPublishEventHandler callBack)
        {
            try
            {
                // register to message received 
                if (null != callBack)
                    MqttMsgPublishReceived += callBack;

                // subscribe to the topic 
                Subscribe(subscribeInfo.Topics, subscribeInfo.QoS);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"[ElaMqttSubscriber]\t[Subscribe]\tException while subscribing to broker (host={BrokerInfo.broker_address}, port={BrokerInfo.broker_port}): {ex.Message}", ex);
            }
        }

        /**
         * \fn Unsubscribe
         * \brief unsubscribe from mqtt flow
         * \param [in] mqttSubscribeInfo : mqtt subscribe informations
         * \param [in] us_topics : target topics for unsubscription
         * \return success or failed
         */
        public bool Unsubscribe(MqttSubscribeInfo mqttSubscribeInfo, MqttMsgPublishEventHandler callBack)
        {
            try
            {
                //unsubscribe
                if (null != callBack)
                    MqttMsgPublishReceived -= callBack;
                Unsubscribe(mqttSubscribeInfo.Topics);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception while trying to unsubscribe from broker (host={BrokerInfo.broker_address}, port={BrokerInfo.broker_port}) : {ex.Message}", ex);
            }
        }
        #endregion
    }
}
