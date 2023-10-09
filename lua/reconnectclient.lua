local serverReconnectEvent = ac.OnlineEvent({ message = ac.StructItem.string(16) }, function(sender, data)
    if data.message:match('ReconnectClients') and (sender and sender.index or -1) == -1 then
        ac.reconnectTo({ carID = ac.getCarID(0) })
        return true
    end
end)
