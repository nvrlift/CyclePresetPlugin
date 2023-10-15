local pngUrl = "http://" .. ac.getServerIP() .. ":" .. ac.getServerPortHTTP() .. "/cyclepreset/reconnecting.png"

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
local color = rgbm(255, 255, 255, 1.0)
local pngSize = vec2(900, 300)
function script.drawUI()
    if drawReconnect then
        if centerPos == nil then
            centerPos = vec2(ac.getUI().windowSize.x / 2, ac.getUI().windowSize.y / 2)
        end
        
        local position = vec2(centerPos.x - (pngSize.x / 2), centerPos.y - (pngSize.y / 2))

        ui.drawImage(pngUrl, position, position + pngSize, color)
    end
end

function sleep(n)  -- seconds
    --[[local t0 = os.clock()
    while os.clock() - t0 <= n do end]]

    local ntime = os.clock() + n -- / 10
    repeat until os.clock() > ntime
end

local waitticks = 10 
function script.update(dt)
    if (drawReconnect == true and pleaseReconnect == false and waitticks > 0) then
        waitticks = waitticks - 1
    elseif (drawReconnect == true and pleaseReconnect == false and waitticks == 0) then
        pleaseReconnect = true
    elseif pleaseReconnect then
        pleaseReconnect = false
        sleep(reconnectDelay)
        -- drawReconnect = false
        ac.reconnectTo({ carID = ac.getCarID(0) })
    end
end
