/* MPU */

//using Vanara.PInvoke;

namespace SharpDune.Audio;

class Controls
{
    internal byte volume;
    internal byte modulation;
    internal byte panpot;
    internal byte expression;
    internal byte sustain;
    internal byte patch_bank_sel;
    internal byte chan_lock;
    internal byte chan_protect;
    internal byte voice_protect;

    internal void SetMembersTo(byte value)
    {
        volume = value;
        modulation = value;
        panpot = value;
        expression = value;
        sustain = value;
        patch_bank_sel = value;
        chan_lock = value;
        chan_protect = value;
        voice_protect = value;
    }
}

class MSData
{
    internal MSData()
    {
        for (var i = 0; i < controls.Length; i++) controls[i] = new Controls();
    }

    internal Memory<byte> EVNT;                                     /*!< Pointer to EVNT position in sound file. */
    internal Array<byte> sound;                                     /*!< Pointer to current position in sound file. */
    internal ushort playing;                                        /*!< status : 0 = SEQ_STOPPED, 1 = SEQ_PLAYING or 2 = SEQ_DONE. */
    internal bool delayedClear;                                     /*!< post_release */
    internal short delay;                                           /*!< Delay before reading next command. interval_cnt */
    internal ushort noteOnCount;                                    /*!< Number of notes currently on. note_count */
    /*ushort variable_0022;*/                                       /*!< vol_error - unused */
    internal ushort globalVolume;                                   /*!< Volume (percent) */
    internal ushort globalVolumeTarget;                             /*!< Volume target */
    internal uint globalVolumeAcc;                                  /*!< Volume accumulator */
    internal uint globalVolumeIncr;                                 /*!< Volume increment per 100us period */
    internal ushort tempoError;                                     /*!< tempo_error */
    internal ushort tempoPercent;                                   /*!< tempo_percent */
    internal ushort tempoTarget;                                    /*!< tempo_target */
    internal uint tempoAcc;                                         /*!< tempo_accum */
    internal uint tempoPeriod;                                      /*!< tempo_period */
    internal ushort beatCount;                                      /*!< beat_count */
    internal ushort measureCount;                                   /*!< measure_count */
    internal ushort timeNumerator;                                  /*!< time_numerator */
    internal uint timeFraction;                                     /*!< time_fraction */
    internal uint beatFraction;                                     /*!< beat_fraction */
    internal uint timePerBeat;                                      /*!< time_per_beat */
    internal Memory<byte>[] forLoopPtrs = new Memory<byte>[4];      /*!< FOR_loop_ptrs pointer to start of FOR loop */
    internal ushort[] forLoopCounters = new ushort[4];              /*!< FOR_loop_cnt */
    internal byte[] chanMaps = new byte[NUM_CHANS];         /*!< ?? Channel mapping. */
    internal Controls[] controls = new Controls[NUM_CHANS]; /*!< ?? */
    internal byte[] noteOnChans = new byte[MAX_NOTES];      /*!< ?? */
    internal byte[] noteOnNotes = new byte[MAX_NOTES];      /*!< ?? */
    internal int[] noteOnDuration = new int[MAX_NOTES];     /*!< ?? */
}

class Mt32Mpu
{
    /* defines from AIL XMIDI.ASM : */
    internal const int NUM_CHANS = 16;
    internal const int MAX_NOTES = 32;

    static readonly MSData[] s_mpu_msdata = new MSData[8];
    static ushort s_mpu_msdataSize;
    static ushort s_mpu_msdataCurrent;

    static readonly Controls[] s_mpu_controls = new Controls[NUM_CHANS];    /* global_controls */
    static readonly byte[] s_mpu_programs = new byte[NUM_CHANS];            /* global_program */
    static readonly ushort[] s_mpu_pitchWheel = new ushort[NUM_CHANS];   /* global_pitch */
    static readonly byte[] s_mpu_noteOnCount = new byte[NUM_CHANS];      /* active_notes */
    static readonly byte[] s_mpu_lockStatus = new byte[NUM_CHANS];      /* bit 7: locked, bit 6: lock-protected */

    static bool s_mpuIgnore;
    static bool s_mpu_initialized;

    static SemaphoreSlim s_mpu_sem;
    //static Kernel32.SafeSemaphoreHandle/*Semaphore*/ s_mpu_sem;
    static Thread s_mpu_thread;
    //static Kernel32.SafeHTHREAD/*Thread*/ s_mpu_thread;

    static uint s_mpu_usec;

    static void MPU_Send(byte status, byte data1, byte data2)
    {
        s_mpuIgnore = true;
        Midi_Send((uint)(status | (data1 << 8) | (data2 << 16)));
        s_mpuIgnore = false;
    }

    static void MPU_StopAllNotes(MSData data)
    {
        byte i;

        for (i = 0; i < MAX_NOTES; i++)
        {
            byte note;
            byte chan;

            chan = data.noteOnChans[i];
            if (chan == 0xFF) continue;

            data.noteOnChans[i] = 0xFF;
            note = data.noteOnNotes[i];
            chan = data.chanMaps[chan];

            /* Note Off */
            MPU_Send((byte)(0x80 | chan), note, 0);
        }

        data.noteOnCount = 0;
    }

    /*
     * XMIDI.ASM - flush_channel_notes
     */
    static void MPU_FlushChannel(byte channel)
    {
        ushort count;
        ushort index = 0;

        count = s_mpu_msdataSize;

        while (count-- != 0)
        {
            MSData data;
            byte i;

            while (s_mpu_msdata[index] == null) index++;

            data = s_mpu_msdata[index];

            if (data.noteOnCount == 0) continue;

            for (i = 0; i < MAX_NOTES; i++)
            {
                byte chan;
                byte note;

                chan = data.noteOnChans[i];
                if (chan != channel) continue;
                data.noteOnChans[i] = 0xFF;

                note = data.noteOnNotes[i];
                chan = data.chanMaps[chan];
                s_mpu_noteOnCount[chan]--;

                /* Note Off */
                MPU_Send((byte)(0x80 | chan), note, 0);

                data.noteOnCount--;
            }
        }
    }

    /*
     * XMIDI.ASM - release_channel
     */
    static void MPU_ReleaseChannel(byte chan)
    {
        if ((s_mpu_lockStatus[chan] & 0x80) == 0) return;

        s_mpu_lockStatus[chan] &= 0x7F;
        s_mpu_noteOnCount[chan] = 0;

        /* Sustain Off */
        MPU_Send((byte)(0xB0 | chan), 64, 0);

        /* All Notes Off */
        MPU_Send((byte)(0xB0 | chan), 123, 0);

        if (s_mpu_controls[chan].volume != 0xFF) MPU_Send((byte)(0xB0 | chan), 7, s_mpu_controls[chan].volume);
        if (s_mpu_controls[chan].modulation != 0xFF) MPU_Send((byte)(0xB0 | chan), 1, s_mpu_controls[chan].modulation);
        if (s_mpu_controls[chan].panpot != 0xFF) MPU_Send((byte)(0xB0 | chan), 10, s_mpu_controls[chan].panpot);
        if (s_mpu_controls[chan].expression != 0xFF) MPU_Send((byte)(0xB0 | chan), 11, s_mpu_controls[chan].expression);
        if (s_mpu_controls[chan].sustain != 0xFF) MPU_Send((byte)(0xB0 | chan), 64, s_mpu_controls[chan].sustain);
        if (s_mpu_controls[chan].patch_bank_sel != 0xFF) MPU_Send((byte)(0xB0 | chan), 114, s_mpu_controls[chan].patch_bank_sel);
        if (s_mpu_controls[chan].chan_lock != 0xFF) MPU_Send((byte)(0xB0 | chan), 110, s_mpu_controls[chan].chan_lock);
        if (s_mpu_controls[chan].chan_protect != 0xFF) MPU_Send((byte)(0xB0 | chan), 111, s_mpu_controls[chan].chan_protect);
        if (s_mpu_controls[chan].voice_protect != 0xFF) MPU_Send((byte)(0xB0 | chan), 112, s_mpu_controls[chan].voice_protect);

        if (s_mpu_programs[chan] != 0xFF) MPU_Send((byte)(0xC0 | chan), s_mpu_programs[chan], 0);

        if (s_mpu_pitchWheel[chan] != 0xFFFF) MPU_Send((byte)(0xE0 | chan), (byte)(s_mpu_pitchWheel[chan] & 0xFF), (byte)(s_mpu_pitchWheel[chan] >> 8));
    }

    /*
     * XMIDI.ASM - reset_sequence
     */
    static void MPU_ResetSequence(MSData data)
    {
        byte chan;

        for (chan = 0; chan < NUM_CHANS; chan++)
        {
            if (data.controls[chan].sustain is not 0xFF and >= 64)
            {
                s_mpu_controls[chan].sustain = 0;
                /* Sustain Off */
                MPU_Send((byte)(0xB0 | chan), 64, 0);
            }

            if (data.controls[chan].chan_lock is not 0xFF and >= 64)
            {
                MPU_FlushChannel(chan);
                MPU_ReleaseChannel(data.chanMaps[chan]);    /* release_channel */
                data.chanMaps[chan] = chan;
            }

            if (data.controls[chan].chan_protect is not 0xFF and >= 64) s_mpu_lockStatus[chan] &= 0xBF;

            if (data.controls[chan].voice_protect is not 0xFF and >= 64) MPU_Send((byte)(0xB0 | chan), 112, 0); /* 112 = VOICE_PROTECT */
        }
    }

    /*
     * XMIDI.ASM - release_seq
     */
    internal static void MPU_ClearData(ushort index)
    {
        MSData data;

        if (index == 0xFFFF) return;
        if (s_mpu_msdata[index] == null) return;

        data = s_mpu_msdata[index];

        if (data.playing == 1)
        {
            data.delayedClear = true;
        }
        else
        {
            s_mpu_msdata[index] = null;
            s_mpu_msdataSize--;
        }
    }

    internal static void MPU_Stop(ushort index)
    {
        MSData data;

        if (index == 0xFFFF) return;
        if (s_mpu_msdata[index] == null) return;

        data = s_mpu_msdata[index];

        if (data.playing != 1) return;

        MPU_StopAllNotes(data);
        MPU_ResetSequence(data);

        data.playing = 0;
    }

    static Memory<byte> MPU_FindSoundStart(Memory<byte> file, ushort index)
    {
        uint total;
        uint header;
        uint size;
        var pointer = 0;

        index++;

        while (true)
        {
            header = Read_BE_UInt32(file.Slice(pointer));
            size = Read_BE_UInt32(file.Slice(pointer + 4));

            if (header != SharpDune.MultiChar[FourCC.CAT] && header != SharpDune.MultiChar[FourCC.FORM]) return null;
            if (Read_BE_UInt32(file.Slice(pointer + 8)) == SharpDune.MultiChar[FourCC.XMID]) break;

            pointer += 8;
            pointer += (int)size;
        }
        total = size - 5;

        if (header == SharpDune.MultiChar[FourCC.FORM]) return (index == 1) ? file.Slice(pointer) : null;

        pointer += 12;

        while (true)
        {
            size = Read_BE_UInt32(file.Slice(pointer + 4));

            if ((Read_BE_UInt32(file.Slice(pointer + 8)) == SharpDune.MultiChar[FourCC.XMID]) && --index == 0) break;

            size += 8;
            total -= size;
            if ((int)total < 0) return null;

            pointer += (int)size;
        }

        return file.Slice(pointer);
    }

    static void MPU_InitData(MSData data)
    {
        byte i;

        for (i = 0; i < 4; i++) data.forLoopCounters[i] = 0xFFFF;

        for (i = 0; i < NUM_CHANS; i++) data.chanMaps[i] = i;

        for (i = 0; i < data.controls.Length; i++) data.controls[i].SetMembersTo(0xFF); //memset(data->controls, 0xFF, sizeof(data->controls));
        Array.Fill<byte>(data.noteOnChans, 0xFF); //memset(data->noteOnChans, 0xFF, sizeof(data->noteOnChans));

        data.delay = 0;
        data.noteOnCount = 0;
        data.globalVolume = 0x5A;       /* 90% */
        data.globalVolumeTarget = 0x5A; /* 90% */
        data.tempoError = 0;
        data.tempoPercent = 0x64;       /* = 100 */
        data.tempoTarget = 0x64;        /* = 100 */
        data.beatCount = 0;
        data.measureCount = 0;
        data.timeNumerator = 4;
        data.timeFraction = 0x208D5;    /* 133333 */
        data.beatFraction = 0x208D5;    /* 133333 */
        data.timePerBeat = 0x7A1200;    /* 8000000 */
    }

    internal static ushort MPU_SetData(Memory<byte> file, ushort index, MSData msdata)
    {
        var data = msdata;
        uint header;
        uint size;
        ushort i;
        var pointer = 0;

        if (file.IsEmpty) return 0xFFFF;

        for (i = 0; i < 8; i++)
        {
            if (s_mpu_msdata[i] == null) break;
        }
        if (i == 8) return 0xFFFF;

        file = MPU_FindSoundStart(file, index);
        if (file.IsEmpty) return 0xFFFF;

        s_mpu_msdata[i] = data;
        data.EVNT = null;

        header = Read_BE_UInt32(file.Slice(pointer));
        size = 12;
        while (header != SharpDune.MultiChar[FourCC.EVNT])
        {
            pointer += (int)size;
            header = Read_BE_UInt32(file.Slice(pointer));
            size = Read_BE_UInt32(file.Slice(pointer + 4)) + 8;
        }

        data.EVNT = file.Slice(pointer);
        data.playing = 0;
        data.delayedClear = false;

        s_mpu_msdataSize++;

        MPU_InitData(data);

        return i;
    }

    internal static void MPU_Play(ushort index)
    {
        MSData data;

        if (index == 0xFFFF) return;

        data = s_mpu_msdata[index];

        if (data.playing == 1) MPU_Stop(index);

        MPU_InitData(data);

        data.sound = new Array<byte>(data.EVNT.Slice(8));

        data.playing = 1;
    }

    /*
     * XMIDI.ASM - XMIDI_volume
     * Apply global volume to all channels
     */
    static void MPU_ApplyVolume(MSData data)
    {
        byte i;

        for (i = 0; i < NUM_CHANS; i++)
        {
            byte volume;

            volume = data.controls[i].volume;
            if (volume == 0xFF) continue;

            /* get scaled volume value, maximum is 127 */
            volume = (byte)Math.Min((volume * data.globalVolume) / 100, 127);

            s_mpu_controls[i].volume = volume;

            if ((s_mpu_lockStatus[i] & 0x80) != 0) continue;

            MPU_Send((byte)(0xB0 | data.chanMaps[i]), 7 /* PART_VOLUME */, volume);
        }
    }

    /*
     * XMIDI.ASM - set_rel_volume
     * Set relative volume
     * @param volume Target volume (%) to reach
     * @param time Time to reach target volume (in milliseconds)
     */
    internal static void MPU_SetVolume(ushort index, ushort volume, ushort time)
    {
        MSData data;
        short diff;

        if (index == 0xFFFF) return;

        data = s_mpu_msdata[index];

        data.globalVolumeTarget = volume;   /* volume target */

        if (time == 0)
        {
            /* immediate */
            data.globalVolume = volume;
            MPU_ApplyVolume(data);
            return;
        }

        diff = (short)(data.globalVolumeTarget - data.globalVolume);
        if (diff == 0) return;

        data.globalVolumeIncr = 10 * (uint)time / (ushort)Math.Abs(diff);   /* volume increment per 100us period */
        if (data.globalVolumeIncr == 0) data.globalVolumeIncr = 1;
        data.globalVolumeAcc = 0;   /* vol_accum */
    }

    internal static ushort MPU_IsPlaying(ushort index)
    {
        MSData data;

        if (index == 0xFFFF) return 0xFFFF;

        data = s_mpu_msdata[index];

        return data.playing;
    }

    internal static ushort MPU_GetDataSize() =>
        1;

    //static uint/*ThreadStatus WINAPI*/ MPU_ThreadProc(IntPtr data)
    static void MPU_ThreadProc()
    {
        s_mpu_sem.Wait(Timeout.Infinite);
        //Thread.Semaphore_Lock(s_mpu_sem);
        while (!s_mpu_sem.Wait(0))
        //while (!Thread.Semaphore_TryLock(s_mpu_sem))
        {
            MSleep((int)(s_mpu_usec / 1000));
            MPU_Interrupt();
        }
        s_mpu_sem.Release(1);
        //Thread.Semaphore_Unlock(s_mpu_sem);

        //return 0;
    }

    static readonly byte[] defaultPrograms = { 68, 48, 95, 78, 41, 3, 110, 122, 255 };
    internal static bool MPU_Init()
    {
        byte i;

        if (!Midi_Init()) return false;

        //s_mpu_sem = Thread.Semaphore_Create(0);
        s_mpu_sem = new SemaphoreSlim(0, 1);

        //s_mpu_thread = Thread.Thread_Create(MPU_ThreadProc, IntPtr.Zero);
        s_mpu_thread = new Thread(MPU_ThreadProc);
        s_mpu_thread.Start();

        s_mpu_msdataSize = 0;
        s_mpu_msdataCurrent = 0;
        Array.Fill(s_mpu_msdata, null, 0, s_mpu_msdata.Length); //memset(s_mpu_msdata, 0, sizeof(s_mpu_msdata));

        var controls = new Controls();
        controls.SetMembersTo(0xFF);
        Array.Fill(s_mpu_controls, controls, 0, s_mpu_controls.Length); //memset(s_mpu_controls, 0xFF, sizeof(s_mpu_controls));
        Array.Fill<byte>(s_mpu_programs, 0xFF, 0, s_mpu_programs.Length); //memset(s_mpu_programs, 0xFF, sizeof(s_mpu_programs));
        Array.Fill<ushort>(s_mpu_pitchWheel, 0xFF, 0, s_mpu_pitchWheel.Length); //memset(s_mpu_pitchWheel, 0xFF, sizeof(s_mpu_pitchWheel));

        Array.Fill<byte>(s_mpu_noteOnCount, 0, 0, s_mpu_noteOnCount.Length); //memset(s_mpu_noteOnCount, 0, sizeof(s_mpu_noteOnCount));
        Array.Fill<byte>(s_mpu_lockStatus, 0, 0, s_mpu_lockStatus.Length); //memset(s_mpu_lockStatus, 0, sizeof(s_mpu_lockStatus));

        s_mpuIgnore = true;
        Midi_Reset();
        s_mpuIgnore = false;

        for (i = 0; i < 9; i++)
        {
            var chan = (byte)(i + 1);

            /* default controller/program change
             * values for startup initialization */
            s_mpu_controls[chan].volume = 127;
            s_mpu_controls[chan].modulation = 0;
            s_mpu_controls[chan].panpot = 64;
            s_mpu_controls[chan].expression = 127;
            s_mpu_controls[chan].sustain = 0;
            s_mpu_controls[chan].patch_bank_sel = 0;
            s_mpu_controls[chan].chan_lock = 0;
            s_mpu_controls[chan].chan_protect = 0;
            s_mpu_controls[chan].voice_protect = 0;

            MPU_Send((byte)(0xB0 | chan), 7, s_mpu_controls[chan].volume);        /* 7 = PART_VOLUME */
            MPU_Send((byte)(0xB0 | chan), 1, s_mpu_controls[chan].modulation);    /* 1 = MODULATION */
            MPU_Send((byte)(0xB0 | chan), 10, s_mpu_controls[chan].panpot);        /* 10 = PANPOT */
            MPU_Send((byte)(0xB0 | chan), 11, s_mpu_controls[chan].expression);    /* 11 = EXPRESSION */
            MPU_Send((byte)(0xB0 | chan), 64, s_mpu_controls[chan].sustain);       /* 64 = SUSTAIN */
            MPU_Send((byte)(0xB0 | chan), 114, s_mpu_controls[chan].patch_bank_sel);/* 114 = PATCH_BANK_SEL */
            MPU_Send((byte)(0xB0 | chan), 110, s_mpu_controls[chan].chan_lock);     /* 110 = CHAN_LOCK */
            MPU_Send((byte)(0xB0 | chan), 111, s_mpu_controls[chan].chan_protect);  /* 111 = CHAN_PROTECT */
            MPU_Send((byte)(0xB0 | chan), 112, s_mpu_controls[chan].voice_protect); /* 112 = VOICE_PROTECT */

            s_mpu_pitchWheel[chan] = 0x4000;
            MPU_Send((byte)(0xE0 | chan), 0x00, 0x40);  /* 0xE0 : Pitch Bend change */

            if (defaultPrograms[i] == 0xFF) continue;
            s_mpu_programs[chan] = defaultPrograms[i];
            MPU_Send((byte)(0xC0 | chan), defaultPrograms[i], 0);   /* 0xC0 : program change */
        }

        s_mpu_initialized = true;
        return true;
    }

    internal static void MPU_Uninit()
    {
        ushort i;

        if (!s_mpu_initialized) return;

        for (i = 0; i < s_mpu_msdataSize; i++)
        {
            if (s_mpu_msdata[i] == null) continue;
            MPU_Stop(i);
            MPU_ClearData(i);
        }

        s_mpuIgnore = true;
        Midi_Reset();

        s_mpu_initialized = false;

        Midi_Uninit();
        s_mpuIgnore = false;

        s_mpu_sem.Dispose();
        //Thread.Semaphore_Destroy(s_mpu_sem);
    }

    internal static void MPU_StartThread(uint usec)
    {
        s_mpu_usec = usec;
        s_mpu_sem.Release(1);
        //Thread.Semaphore_Unlock(s_mpu_sem);
    }

    internal static void MPU_StopThread()
    {
        s_mpu_sem.Release(1);
        //Thread.Semaphore_Unlock(s_mpu_sem);
        s_mpu_thread.Join();
        //Thread.Thread_Wait(s_mpu_thread, out uint _);
    }

    static bool locked;
    static readonly byte[] buffer = new byte[320];
    static void MPU_Interrupt()
    {
        ushort count;

        if (s_mpuIgnore) return;
        if (locked) return;
        locked = true;

        unchecked { s_mpu_msdataCurrent = (ushort)-1; }
        count = s_mpu_msdataSize;
        while (count-- != 0)
        {
            ushort index;
            MSData data;

            do
            {
                s_mpu_msdataCurrent++;
                index = s_mpu_msdataCurrent;
            } while (index < s_mpu_msdata.Length && s_mpu_msdata[index] == null);

            if (index >= s_mpu_msdata.Length) break;

            data = s_mpu_msdata[index];

            if (data.playing != 1) continue;

            data.tempoError += data.tempoPercent;

            while (data.tempoError >= 0x64)
            {   /* 0x64 == 100 */
                uint value;

                data.tempoError -= 0x64;   /* 0x64 == 100 */

                value = data.beatFraction;
                value += data.timeFraction;

                if ((int)value >= (int)data.timePerBeat)
                {
                    value -= data.timePerBeat;
                    if (++data.beatCount >= data.timeNumerator)
                    {
                        data.beatCount = 0x0;
                        data.measureCount++;
                    }
                }

                data.beatFraction = value;

                /* Handle note length */
                unchecked { index = (ushort)-1; }
                while (data.noteOnCount != 0)
                {
                    ushort chan;
                    byte note;
                    while (++index < 0x20)
                    {
                        if (data.noteOnChans[index] != 0xFF) break;
                    }
                    if (index == 0x20) break;

                    if (--data.noteOnDuration[index] >= 0) continue;

                    chan = data.chanMaps[data.noteOnChans[index]];
                    data.noteOnChans[index] = 0xFF;
                    note = data.noteOnNotes[index];
                    s_mpu_noteOnCount[chan]--;

                    /* Note Off */
                    MPU_Send((byte)(0x80 | chan), note, 0);

                    data.noteOnCount--;
                }

                if (--data.delay <= 0)
                {
                    do
                    {
                        byte status;
                        byte chan;
                        byte data1;
                        byte data2;
                        ushort nb;

                        status = data.sound.Curr;

                        if (status < 0x80)
                        {
                            /* Set a delay before next command. */
                            data.sound++;
                            data.delay = status;
                            break;
                        }

                        chan = (byte)(status & 0xF);
                        data1 = data.sound[data.sound.Ptr + 1];
                        data2 = data.sound[data.sound.Ptr + 2];

                        switch (status & 0xF0)
                        {
                            case 0xF0:  /* System */
                                if (chan == 0xF)
                                {
                                    /* 0xFF Meta event */
                                    nb = MPU_XMIDIMeta(data);
                                }
                                else if (chan == 0)
                                {
                                    /* System Exclusive */
                                    int i;
                                    /* decode XMID variable len */
                                    i = 1;
                                    nb = 0;
                                    do
                                    {
                                        nb = (ushort)((nb << 7) | (data.sound[data.sound.Ptr + i] & 0x7F));
                                    } while ((data.sound[data.sound.Ptr + i++] & 0x80) == 0x80);
                                    buffer[0] = status;
                                    Debug.Assert(nb < buffer.Length);
                                    data.sound.Arr.Slice(data.sound.Ptr + i, nb).CopyTo(new Memory<byte>(buffer, 1, nb)); //memcpy(buffer + 1, data.sound + i, nb);
                                    Midi_Send_String(buffer, (ushort)(nb + 1));
                                    nb += (ushort)i;
                                }
                                else
                                {
                                    Trace.WriteLine($"ERROR: Status = {status:X2}");
                                    nb = 1;
                                }
                                break;
                            case 0xE0:  /* Pitch Bend change */
                                s_mpu_pitchWheel[chan] = (ushort)((data2 << 8) + data1);
                                if ((s_mpu_lockStatus[chan] & 0x80) == 0)
                                {
                                    MPU_Send((byte)(status | data.chanMaps[chan]), data1, data2);
                                }
                                nb = 0x3;
                                break;
                            case 0xD0:  /* Channel Pressure / aftertouch */
                                if ((s_mpu_lockStatus[chan] & 0x80) == 0)
                                {
                                    MPU_Send((byte)(status | data.chanMaps[chan]), data1, 0);
                                }
                                nb = 0x2;
                                break;
                            case 0xC0:  /* Program Change */
                                s_mpu_programs[chan] = data1;
                                if ((s_mpu_lockStatus[chan] & 0x80) == 0)
                                {
                                    MPU_Send((byte)(status | data.chanMaps[chan]), data1, 0);
                                }
                                nb = 0x2;
                                break;
                            case 0xB0:  /* Control Change */
                                MPU_Control(data, chan, data1, data2);
                                nb = 0x3;
                                break;
                            case 0xA0:  /* Polyphonic key pressure / aftertouch */
                                if ((s_mpu_lockStatus[chan] & 0x80) == 0)
                                {
                                    MPU_Send((byte)(status | data.chanMaps[chan]), data1, data2);
                                }
                                nb = 0x3;
                                break;
                            default:    /* 0x80 Note Off / 0x90 Note On */
                                nb = MPU_NoteOn(data);
                                break;
                        }

                        data.sound += nb;
                    } while (data.playing == 1);
                }
                if (data.playing != 1) break;
            }

            if (data.playing != 1) continue;

            if (data.tempoPercent != data.tempoTarget)
            {
                uint v;
                ushort i;

                v = data.tempoAcc;
                v += 0x53;  /* 0x53 = 83 = 10000 / 120 */
                i = 0xFFFF;

                do
                {
                    i++;
                    data.tempoAcc = v;
                    v -= data.tempoPeriod;
                } while ((int)v >= 0);

                if (i != 0)
                {
                    data.tempoPercent = data.tempoPercent >= data.tempoTarget
                        ? (ushort)Math.Max(data.tempoPercent - i, data.tempoTarget)
                        : (ushort)Math.Min(data.tempoPercent + i, data.tempoTarget);
                }
            }

            if (data.globalVolume != data.globalVolumeTarget)
            {
                uint v;
                ushort i;

                v = data.globalVolumeAcc;
                v += 0x53;  /* 0x53 = 83 = 10000 / 120 */
                i = 0xFFFF;

                do
                {
                    i++;
                    data.globalVolumeAcc = v;
                    v -= data.globalVolumeIncr;
                } while ((int)v >= 0);

                if (i != 0)
                {
                    data.globalVolume = data.globalVolume >= data.globalVolumeTarget
                        ? (ushort)Math.Max(data.globalVolume - i, data.globalVolumeTarget)
                        : (ushort)Math.Min(data.globalVolume + i, data.globalVolumeTarget);
                    MPU_ApplyVolume(data);
                }
            }
        }

        locked = false;

        return;
    }

    static ushort MPU_NoteOn(MSData data)
    {
        byte chan;
        byte note;
        byte velocity;
        ushort len = 0;
        uint duration = 0;
        byte i;

        chan = (byte)(data.sound[data.sound.Ptr + len++] & 0x0F);
        note = data.sound[data.sound.Ptr + len++];
        velocity = data.sound[data.sound.Ptr + len++];

        /* decode variable length duration */
        do
        {
            duration = (duration << 7) | (uint)(data.sound[data.sound.Ptr + len] & 0x7F);
        } while ((data.sound[data.sound.Ptr + len++] & 0x80) == 0x80);

        if ((s_mpu_lockStatus[chan] & 0x80) != 0) return len;

        for (i = 0; i < MAX_NOTES; i++)
        {
            if (data.noteOnChans[i] == 0xFF)
            {
                data.noteOnCount++;
                break;
            }
        }
        if (i == MAX_NOTES) i = 0;

        data.noteOnChans[i] = chan;
        data.noteOnNotes[i] = note;
        data.noteOnDuration[i] = (int)(duration - 1);

        chan = data.chanMaps[chan];
        s_mpu_noteOnCount[chan]++;

        /* Note On */
        MPU_Send((byte)(0x90 | chan), note, velocity);

        return len;
    }

    /* XMIDI.ASM - XMIDI_control procedure */
    static void MPU_Control(MSData data, byte chan, byte control, byte value)
    {
        switch (control)
        {
            case 1: /* MODULATION */
                s_mpu_controls[chan].modulation = value;
                data.controls[chan].modulation = value;
                if ((s_mpu_lockStatus[chan] & 0x80) == 0) MPU_Send((byte)(0xB0 | data.chanMaps[chan]), control, value);
                break;

            case 7: /* PART_VOLUME / Channel Volume */
                if (data.globalVolume == 100) break;
                value = (byte)Math.Min(data.globalVolume * value / 100, 127);
                s_mpu_controls[chan].volume = value;
                if ((s_mpu_lockStatus[chan] & 0x80) == 0) MPU_Send((byte)(0xB0 | data.chanMaps[chan]), control, value);
                break;

            case 10:    /* PANPOT */
                s_mpu_controls[chan].panpot = value;
                data.controls[chan].panpot = value;
                if ((s_mpu_lockStatus[chan] & 0x80) == 0) MPU_Send((byte)(0xB0 | data.chanMaps[chan]), control, value);
                break;

            case 11:    /* EXPRESSION */
                s_mpu_controls[chan].expression = value;
                data.controls[chan].expression = value;
                if ((s_mpu_lockStatus[chan] & 0x80) == 0) MPU_Send((byte)(0xB0 | data.chanMaps[chan]), control, value);
                break;

            case 64:    /* SUSTAIN */
                s_mpu_controls[chan].sustain = value;
                data.controls[chan].sustain = value;
                if ((s_mpu_lockStatus[chan] & 0x80) == 0) MPU_Send((byte)(0xB0 | data.chanMaps[chan]), control, value);
                break;

            case 110:   /* CHAN_LOCK */
                s_mpu_controls[chan].chan_lock = value;
                data.controls[chan].chan_lock = value;
                Debug.WriteLine($"DEBUG: CHAN_LOCK : chan={chan} {control} {value}");
                if (value < 64)
                {
                    /* unlock */
                    MPU_FlushChannel(chan);
                    MPU_ReleaseChannel(data.chanMaps[chan]);   /* release channel */
                    data.chanMaps[chan] = chan;
                }
                else
                {
                    /* lock */
                    var newChan = MPU_LockChannel();  /* lock new channel and map to current channel in sequence */
                    if (newChan == 0xFF) newChan = chan;

                    data.chanMaps[chan] = newChan;
                }
                break;

            case 111:   /* CHAN_PROTECT */
                s_mpu_controls[chan].chan_protect = value;
                data.controls[chan].chan_protect = value;
                if (value >= 64) s_mpu_lockStatus[chan] |= 0x40;
                break;

            case 112:   /* VOICE_PROTECT */
                s_mpu_controls[chan].voice_protect = value;
                data.controls[chan].voice_protect = value;
                if ((s_mpu_lockStatus[chan] & 0x80) == 0) MPU_Send((byte)(0xB0 | data.chanMaps[chan]), control, value);
                break;

            case 114:   /* PATCH_BANK_SEL */
                s_mpu_controls[chan].patch_bank_sel = value;
                data.controls[chan].patch_bank_sel = value;
                if ((s_mpu_lockStatus[chan] & 0x80) == 0) MPU_Send((byte)(0xB0 | data.chanMaps[chan]), control, value);
                break;

            case 115:   /* INDIRECT_C_PFX */
                Debug.Assert(false); /* Not decompiled code */
                break;

            case 116:
                {   /* FOR_LOOP */
                    byte i;

                    for (i = 0; i < 4; i++)
                    {
                        if (data.forLoopCounters[i] == 0xFFFF)
                        {
                            data.forLoopCounters[i] = value;
                            data.forLoopPtrs[i] = data.sound.Arr.Slice(data.sound.Ptr);
                            break;
                        }
                    }
                }
                break;

            case 117:
                {   /* NEXT_LOOP */
                    byte i;

                    if (value < 64) break;

                    for (i = 0; i < 4; i++)
                    {
                        if (data.forLoopCounters[3 - i] != 0xFFFF)
                        {
                            if (data.forLoopCounters[3 - i] != 0 && --data.forLoopCounters[3 - i] == 0)
                            {
                                data.forLoopCounters[3 - i] = 0xFFFF;
                                break;
                            }
                            data.sound.Arr = data.forLoopPtrs[3 - i];
                            data.sound.Ptr = 0;
                            break;
                        }
                    }
                }
                break;

            case 118:   /* CLEAR_BEAT_BAR */
            case 119:   /* CALLBACK_TRIG */
                Debug.Assert(false); /* Not decompiled code */
                break;

            default:
                Debug.WriteLine($"DEBUG: MPU_Control() {data.sound.Curr:X2} {control:X2} {value:X2}   control={control}");
                if ((s_mpu_lockStatus[chan] & 0x80) == 0) MPU_Send((byte)(0xB0 | data.chanMaps[chan]), control, value);
                break;
        }
    }

    /*
     * XMIDI.ASM - lock_channel
     * find highest channel # w/lowest note activity
     * return 0xFF if no channel available for locking
     */
    static byte MPU_LockChannel()
    {
        byte i;
        byte chan = 0xFF;
        byte flag = 0xC0;
        byte min = 0xFF;

        while (true)
        {
            for (i = 0; i < NUM_CHANS; i++)
            {
                if ((s_mpu_lockStatus[15 - i] & flag) == 0 && s_mpu_noteOnCount[15 - i] < min)
                {
                    min = s_mpu_noteOnCount[15 - i];
                    chan = (byte)(15 - i);
                }
            }
            if (chan != 0xFF) break;
            if (flag == 0x80) return chan;

            flag = 0x80;    /* if no channels available for locking, ignore lock protection & try again */
        }

        /* Sustain Off */
        MPU_Send((byte)(0xB0 | chan), 64, 0);

        MPU_FlushChannel(chan);

        s_mpu_noteOnCount[chan] = 0;
        s_mpu_lockStatus[chan] |= 0x80;

        return chan;
    }

    static readonly byte[] rates = { 24, 25, 30, 30 };
    /*
     * XMIDI.ASM - XMIDI_meta
     */
    static ushort MPU_XMIDIMeta(MSData data)
    {
        byte type;
        ushort len;
        ushort data_len = 0;

        type = data.sound[data.sound.Ptr + 1];
        len = 2;

        /* decode variable length */
        do
        {
            data_len = (ushort)((data_len << 7) | (data.sound[data.sound.Ptr + len] & 0x7F));
        } while ((data.sound[data.sound.Ptr + len++] & 0x80) == 0x80);

        switch (type)
        {
            case 0x2F:  /* End of track / end sequence */
                MPU_ResetSequence(data);    /* reset_sequence */

                data.playing = 2; /* 2 = SEQ_DONE */
                if (data.delayedClear) MPU_ClearData(s_mpu_msdataCurrent); /* release-on-completion pending => release_seq */
                break;

            case 0x58:
                {   /* time sig */
                    sbyte mul;

                    data.timeNumerator = data.sound[data.sound.Ptr + len];
                    mul = (sbyte)(data.sound[data.sound.Ptr + len + 1] - 2);

                    data.timeFraction = mul < 0 ? (uint)(133333 >> -mul) : (uint)(133333 << mul);

                    data.beatFraction = data.timeFraction;
                }
                break;

            case 0x51:  /* TEMPO meta-event */
                data.timePerBeat = (uint)((data.sound[data.sound.Ptr + len] << 20) | (data.sound[data.sound.Ptr + len + 1] << 12) | (data.sound[data.sound.Ptr + len + 2] << 4));
                break;

            case 0x59:  /* Key signature : 1st byte = flats/sharps, 2nd byte = major(0)/minor(1) */
                Debug.WriteLine($"DEBUG: MPU_XMIDIMeta() IGNORING key signature : {data.sound[data.sound.Ptr + len]:X2} {data.sound[data.sound.Ptr + len + 1]:X2}");
                break;

            case 0x21:  /* Midi Port */
                Debug.WriteLine($"DEBUG: MPU_XMIDIMeta() IGNORING MIDI Port : {data.sound[data.sound.Ptr + len]:X2}");
                break;

            case 0x54:  /* SMPTE Offset */
                Debug.WriteLine($"DEBUG: MPU_XMIDIMeta() IGNORING SMPTE Offset : {rates[(data.sound[data.sound.Ptr + len] >> 5) & 3]}fps {data.sound[data.sound.Ptr + len] & 31}:{data.sound[data.sound.Ptr + len + 1]:D2}:{data.sound[data.sound.Ptr + len + 2]:D2} {data.sound[data.sound.Ptr + len + 3]}.{data.sound[data.sound.Ptr + len + 4]:D3}");
                break;

            case 0x06:  /* Marker (text) */
                Debug.WriteLine($"DEBUG: MPU_XMIDIMeta() IGNORING Marker '{SharpDune.Encoding.GetString(data.sound.Arr.Span.Slice(data.sound.Ptr + len, data_len))}'");
                break;

            default:
                {
                    int i;
                    Trace.WriteLine($"WARNING: MPU_XMIDIMeta() type={type:X2} len={len}");
                    Trace.WriteLine("WARNING:   ignored data :");
                    for (i = 0; i < len + data_len; i++) Trace.WriteLine($"WARNING:  {data.sound[data.sound.Ptr + i]:X2}");
                    Trace.WriteLine('\n');
                }
                break;
        }

        return (ushort)(len + data_len);
    }
}
