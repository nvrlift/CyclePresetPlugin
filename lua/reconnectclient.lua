local flagsToDraw = {}
local serverReconnectEvent = ac.OnlineEvent(
    {
        ac.StructItem.key("reconnectClientPacket"),
        time = ac.StructItem.uint16(),
        message = ac.StructItem.string(16)
    }, function(sender, data)
        if sender ~= nil then return end
        if data.message:match("ReconnectClients") then

            flagsToDraw = {}

            for flag, imagePath in pairs(flagImages) do
                if bit.band(message.flags, flag) ~= 0 then
                    table.insert(flagsToDraw, "RECONNECTING IN " .. message.time .. " SECONDS...")
                end
            end
            return true
        end
    end)

local centerPos = nil
function script.drawUI()
    if #flagsToDraw > 0 then
        if centerPos == nil then
            centerPos = vec2(ac.getUI().windowSize.x / 2, 100)
        end

        for i, text in ipairs(flagsToDraw) do

            ui.beginTransparentWindow("reconnectClient", centerPos, vec2(400 * 0.5, 400 * 0.5))
            ui.beginOutline()

            ui.pushFont(ui.Font.Huge)

            ui.textColored(text, rgbm.colors.red)

            ui.popFont()
            ui.endOutline(rgbm.colors.black)

            ui.endTransparentWindow()
        end

        sleep(time)
        ac.reconnectTo({ carID = ac.getCarID(0) })
    end
end

function sleep(s)
    local ntime = os.clock() + s / 10
    repeat until os.clock() > ntime
end

