local pleaseReconnect = false
local drawReconnect = false
local reconnectDelay = 0
local reconnectClientEvent = ac.OnlineEvent(
    { -- DONT CHANGE THIS STRUCT OR YOU NEED TO GET A NEW ID FOR THE PACKET.CS
        ac.StructItem.key("reconnectClient"), 
        time = ac.StructItem.uint16()
    }, function(sender, message)
        if sender ~= nil then return end
        
        -- pleaseReconnect = true
        drawReconnect = true
        reconnectDelay = message.time
    end)

local centerPos = nil
function script.drawUI()
    if drawReconnect then
        if centerPos == nil then
            centerPos = vec2((ac.getUI().windowSize.x - 800) / 2, ac.getUI().windowSize.y / 2 - 100)
        end
        
        ui.beginTransparentWindow("reconnectClient", centerPos, vec2(800, 200))
        ui.beginOutline()

        ui.pushFont(ui.Font.Huge)

        ui.textColored("RECONNECTING IN " .. reconnectDelay .. " SECONDS...", rgbm.colors.red)

        ui.popFont()
        ui.endOutline(rgbm.colors.black)

        ui.endTransparentWindow()
    end
end

function sleep(n)  -- seconds
    --[[local t0 = os.clock()
    while os.clock() - t0 <= n do end]]

    local ntime = os.clock() + n -- / 10
    repeat until os.clock() > ntime
end

function script.update(dt)
    if (drawReconnect == true and pleaseReconnect == false) then
        pleaseReconnect = true
    elseif pleaseReconnect then
        pleaseReconnect = false
        sleep(reconnectDelay)
        -- drawReconnect = false
        ac.reconnectTo({ carID = ac.getCarID(0) })
    end
end
