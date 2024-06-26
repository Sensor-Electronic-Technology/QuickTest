﻿using EpiData.Data.Models.Epi.Enums;
using FastEndpoints;
using QuickTest.Data.Contracts.Requests.Post;
using QuickTest.Data.Contracts.Responses;
using QuickTest.Data.Contracts.Responses.Post;
using QuickTest.Data.DataTransfer;
using QuickTest.Data.Models.Wafers;

namespace QuickTest.Data.Mappers;

public class WaferPadMapper:Mapper<CreateWaferPadRequest,CreateWaferPadResponse,WaferPad> {
    public override WaferPad ToEntity(CreateWaferPadRequest r) {
        WaferPad pad = new WaferPad();
        pad.PadLocation = r.PadLocation;
        pad.PadNumber = r.PadNumber;
        pad.SvgObject = r.PadMapDefinition;
        pad.WaferArea = r.WaferArea;
        pad.WaferSize = r.WaferSize;
        if (r.WaferArea.Value == WaferArea.Center) {
            pad.Identifier = $"{r.PadLocation.Value}";
        } else {
            pad.Identifier = $"{r.PadLocation.Value}{r.PadNumber}-{r.WaferArea.Value}";
        }
        return pad;
    }

    public override CreateWaferPadResponse FromEntity(WaferPad entity)=> new() {
        WaferPad = new WaferPadDto(){
            Identifier = entity.Identifier,
            PadNumber = entity.PadNumber,
            SvgObject = entity.SvgObject,
            WaferArea = entity.WaferArea,
            WaferSize = entity.WaferSize,
            PadLocation = entity.PadLocation
        }
    };

    public override WaferPad UpdateEntity(CreateWaferPadRequest r, WaferPad e) {
        if (r.WaferArea.Value == WaferArea.Center) {
            e.Identifier = $"{r.PadLocation.Value}";
        } else {
            e.Identifier = $"{r.PadLocation.Value}{r.PadNumber}-{r.WaferArea.Value}";
        }
        e.PadNumber = r.PadNumber;
        e.SvgObject = r.PadMapDefinition;
        e.WaferArea = r.WaferArea;
        e.WaferSize = r.WaferSize;
        e.PadLocation = r.PadLocation;
        return e;
    }
}