sampler2D inputSource : register(S0);

float4 AmbientColor : register(C0);
float3 LightPos : register(C1);
float3 LightPos2 : register(C2);

float4 LightColor : register(C3);
float4 LightColor2 : register(C4);

float4 main(float2 uv : TEXCOORD) : COLOR
{
		float4 texCol = tex2D(inputSource, uv);
		
		float3 colrgb = texCol.rgb;
    float greycolor = dot(colrgb, float3(0.3, 0.59, 0.11));
    texCol.rgb = lerp(dot(greycolor, float3(0.3, 0.59, 0.11)), colrgb, 0.0);

    //Light 1
    float  lightRadius = 0.5f;
    
    float  lightIntensity = 1.75f;

    float dist = distance(LightPos, uv);
    float dist2 = distance(LightPos2, uv);
    
    
    float4 addcolor = saturate((lightRadius-dist)*lightIntensity)*LightColor;
		addcolor += saturate((lightRadius-dist2)*lightIntensity)*LightColor2;
		
		//Ambient intensity * ambient color
    float4 color = (0.3 * AmbientColor);
		color += addcolor;

    texCol = saturate(color) * texCol + (texCol * 0.2);
		texCol.a = 1;

    return texCol;
}