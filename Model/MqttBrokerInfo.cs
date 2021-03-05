using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

/**
 * \namespace Wirepas_ELA_Mesh_Sample.Model
 * \brief namespace dedicated to the common models
 */
namespace Wirepas_ELA_Mesh_Sample.Model
{
    /**
     * \class MqttBrokerInfo 
     * \brief centralize all the information necessary to instanciate a MQTT communication
     */
    public class MqttBrokerInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /** \brief internal address / hostname definition */
        private string m_Address = string.Empty;

        /** \brief internal port definition */
        private int m_Port;

        #region accessors
        public string broker_address
        {
            get => m_Address;
            set { m_Address = value; OnPropertyChanged(); }
        }
        public int broker_port
        {
            get => m_Port;
            set { m_Port = value; OnPropertyChanged(); }
        }
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        #endregion

        /** \brief constructor */
        public MqttBrokerInfo() { }

        /** 
         * \brief constructor 
         * \param [in] brokerAddress : target broker address
         * \param [in] brokerPort : target connection port 
         */
        public MqttBrokerInfo(string brokerAddress, int brokerPort)
        {
            broker_address = brokerAddress;
            broker_port = brokerPort;
        }

        /** 
         * \brief constructor 
         * \param [in] brokerAddress : target broker address
         * \param [in] brokerPort : target connection port 
         * \param [in] login : mqtt login (not use in this sample)
         * \param [in] password : mqtt password (not use in this sample)
         */
        public MqttBrokerInfo(string brokerAddress, int brokerPort, string login, string password)
        {
            this.broker_address = brokerAddress;
            this.broker_port = brokerPort;
            this.Login = login;
            this.Password = password;
        }

        /**
         * \fn GetKey
         * \brief getter on a specific key to indentifiate broker
         */
        public string GetKey()
        {
            return $"{broker_address};{Login}";
        }

        /**
         * \fn OnPropertyChanged
         * \brief notify that a property has change for this object
         * \param [in] propertyName : target property name for notification
         */
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
