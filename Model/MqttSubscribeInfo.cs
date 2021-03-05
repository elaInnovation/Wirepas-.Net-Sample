using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * \namespace Wirepas_ELA_Mesh_Sample.Model
 * \brief namespace dedicated to the common models
 */
namespace Wirepas_ELA_Mesh_Sample.Model
{
    /**
     * \class MqttSubscribeInfo
     * \brief Mqtt Subscribe Information Model
     */
    public class MqttSubscribeInfo
    {

        /** \brief constructor */
        public MqttSubscribeInfo() { }

        /** 
         * \brief constructor 
         * \param [in] topics : arraoy of target topics definition for subscription
         * \param [in] QoS : array of quality of service associated for this topic
         */
        public MqttSubscribeInfo(string[] topics, byte[] QoS)
        {
            this.Topics = topics;
            this.QoS = QoS;
        }

        #region accessors
        public string[] Topics { get; set; } = new string[] { };
        public byte[] QoS { get; set; } = new byte[] { };
        #endregion
    }
}
