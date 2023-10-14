local pleaseReconnect = false
local reconnectDelay = 0
local reconnectClientEvent = ac.OnlineEvent(
    { -- DONT CHANGE THIS STRUCT OR YOU NEED TO GET A NEW ID FOR THE PACKET.CS
        ac.StructItem.key("reconnectClient"), 
        time = ac.StructItem.uint16()
    }, function(sender, message)
        if sender ~= nil then return end
        
        pleaseReconnect = true
        reconnectDelay = message.time
    end)

local centerPos = nil
function script.drawUI()
    if pleaseReconnect then
        ac.sendChatMessage("pleaseReconnect script.drawUI()")
        if centerPos == nil then
            centerPos = vec2(ac.getUI().windowSize.x / 2, 100)
        end

        ui.beginTransparentWindow("reconnectClient", centerPos, vec2(400 * 0.5, 400 * 0.5))
        ui.beginOutline()

        ui.pushFont(ui.Font.Huge)

        ui.textColored("RECONNECTING IN " .. reconnectDelay .. " SECONDS...", rgbm.colors.red)

        ui.popFont()
        ui.endOutline(rgbm.colors.black)

        ui.endTransparentWindow()
    end
end

function script.update(dt)
    if pleaseReconnect then
        pleaseReconnect = false
        ac.sendChatMessage("pleaseReconnect script.update(dt)")
        
        worker.sleep(reconnectDelay)
        ac.reconnectTo({ carID = ac.getCarID(0) })
    end
end
