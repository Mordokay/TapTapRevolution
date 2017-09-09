﻿// <copyright file="BehaviorUtils.cs" company="Google Inc.">
// Copyright (C) 2014 Google Inc.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

namespace QuizRacer.Behaviors
{
    using UnityEngine;

    public static class BehaviorUtils
    {
        public static void MakeVisible(GameObject obj, bool visible)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null && renderer.enabled != visible)
            {
                renderer.enabled = visible;
            }
            int childCount = obj.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;
                MakeVisible(child, visible);
            }
        }
    }
}
